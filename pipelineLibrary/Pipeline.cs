using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace pipelineLibrary
{
    public class Pipeline
    {
        StreamWriter writer;

        int Circle;

        public Memory Mem;
        public int CodeStart = 0x400;
        public int CodeMaxLen = 0x400;
        public int CodeLen;

        public Registerfile rf;
        public ControlCode CC;
        public ControlLogic CL;

        public FetchStage f;
        public DecodeStage d;
        public ExecuteStage e;
        public MemoryStage m;

        public FetchRegister F;
        public DecodeRegister D;
        public ExecuteRegister E;
        public MemoryRegister M;
        public WritebackRegister W;

        public void Init(string inPath, string outPath = "output.txt")
        {
            Mem = new Memory(CodeStart + CodeMaxLen);
            StreamReader IStream = new StreamReader(inPath);
            BinaryReader reader = new BinaryReader(IStream.BaseStream);
            CodeLen = reader.Read(Mem.Data, CodeStart, CodeMaxLen);
            IStream.Close();


            writer = File.CreateText(outPath);

            Circle = 0;

            CL = new ControlLogic();
            CC = new ControlCode();
            rf = new Registerfile();


            f = new FetchStage();
            d = new DecodeStage();
            e = new ExecuteStage();
            m = new MemoryStage();

            F = new FetchRegister();
            D = new DecodeRegister();
            E = new ExecuteRegister();
            M = new MemoryRegister();
            W = new WritebackRegister();

            F.predPC = CodeStart;
            rf.Data[ConstVar.RESP] = CodeStart;

            f.F = F;
            f.Mem = Mem;
            d.D = D;
            d.rf = rf;
            e.E = E;
            e.CC = CC;
            m.M = M;
            m.Mem = Mem;

            CL.set(D, d, E, e, M);

         
        }

        public bool step()
        {
            try
            {
                m.executeStage();
            }
            catch (Exception exp)
            {
                CloseOutput();
                return false;
            }
            e.executeStage();
            d.executeStage();
            d.SelFwdA(e, E, m, M, W);
            d.FwdB(e, E, m, M, W);
            f.SelectPC(M, W);
            f.executeStage();

            f.translateIns();

            F.stall = CL.Fstall();
            D.stall = CL.Dstall();
            D.bubble = CL.Dbubble();
            E.bubble = CL.Ebubble();

            try
            {
                m.writeMemory();
            }
            catch (Exception exp)
            {
                CloseOutput();
                return false;
            }
            d.writeBack(W);

            F.init(f);
            D.init(f);
            E.init(d);
            M.init(e);
            W.init(m);

            Circle++;
            output();

            if (W.Halt())
            {
                CloseOutput();
                return false;
            }

            if (W.Instrvalid())
            {
                CloseOutput();
                return false;
            }

            return true;
        }

        public void Run()
        {
            while (step()) ;
            CloseOutput();
        }

        public void CloseOutput()
        {
            writer.Close();
        }

        public void PrepareOutput()
        {
            
        }

        public void output()
        {
            writer.WriteLine("Circle_" + Circle);
            writer.WriteLine("--------------------");
            F.output(writer);
            D.output(writer);
            E.output(writer);
            M.output(writer);
            W.output(writer);
        }
    }
}
