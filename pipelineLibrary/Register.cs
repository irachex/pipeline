using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace pipelineLibrary
{
    public class FetchRegister
    {
        public int predPC;
        public bool stall;

        public void initRegister(int CodeStart)
        {
            predPC = CodeStart;
        }

        public void init(FetchStage f)
        {
            if (!stall)
                predPC = f.predPC;
        }

        public void output(StreamWriter writer)
        {
            writer.WriteLine("FETCH:");
            writer.WriteLine("\tF_predPC \t= 0x" + predPC.ToString("x"));
            writer.WriteLine();
        }
    }


    public class MemoryRegister
    {

        public int icode;
        public int valE;
        public int valA;
        public int dstE;
        public int dstM;
        public bool Bch;
        public String Ins;

        public void init(ExecuteStage e)
        {
            icode = e.icode;
            valE = e.valE;
            valA = e.valA;
            dstE = e.dstE;
            dstM = e.dstM;
            Bch = e.Bch;
            Ins = e.Ins;
        }

        public void output(StreamWriter writer)
        {
            writer.WriteLine("MEMORY:");
            writer.WriteLine("\tM_icode \t= " + icode);
            if (Bch)
                writer.WriteLine("\tM_Bch \t\t= 1");
            else
                writer.WriteLine("\tM_Bch \t\t= 0");

            writer.WriteLine("\tM_valE \t\t= 0x" + valE.ToString("x"));
            writer.WriteLine("\tM_valA \t\t= 0x" + valA.ToString("x"));
            writer.WriteLine("\tM_dstE	\t= " + dstE);
            writer.WriteLine("\tM_dstM	\t= " + dstM);
            writer.WriteLine();
        }
    }


    public class ExecuteRegister
    {

        public int icode;
        public int ifun;
        public int valC;
        public int valA;
        public int valB;
        public int dstE;
        public int dstM;
        public int srcA;
        public int srcB;
        public bool bubble;
        public String Ins;

        public void init(DecodeStage d)
        {

            if (bubble)
            {
                icode = 0;
                ifun = 0;
                valC = 0;
                valA = 0;
                valB = 0;
                dstE = 0;
                dstM = 0;
                srcA = 0;
                srcB = 0;
                Ins = "bubble";
            }
            else
            {
                icode = d.icode;
                ifun = d.ifun;
                valC = d.valC;
                valA = d.valA;
                valB = d.valB;
                dstE = d.dstE;
                dstM = d.dstM;
                srcA = d.srcA;
                srcB = d.srcB;
                Ins = d.Ins;
            }

        }

        public void output(StreamWriter writer)
        {
            writer.WriteLine("EXECUTE:");
            writer.WriteLine("\tE_icode \t= " + icode);
            writer.WriteLine("\tE_ifun \t\t= " + ifun);
            writer.WriteLine("\tE_valC \t\t= 0x" + valC.ToString("x"));
            writer.WriteLine("\tE_valA \t\t= 0x" + valA.ToString("x"));
            writer.WriteLine("\tE_valB \t\t= 0x" + valB.ToString("x"));
            writer.WriteLine("\tE_dstE	\t= " + dstE);
            writer.WriteLine("\tE_dstM	\t= " + dstM);
            writer.WriteLine("\tE_srcA	\t= " + srcA);
            writer.WriteLine("\tE_srcB	\t= " + srcB);
            writer.WriteLine();
        }

    }



    public class DecodeRegister
    {

        public int icode;
        public int ifun;
        public int rA;
        public int rB;
        public int valC;
        public int valP;
        public bool stall;
        public bool bubble;
        public String Ins;

        public void init(FetchStage f)
        {

            if (!stall)
            {
                if (bubble)
                {
                    icode = 0;
                    ifun = 0;
                    rA = 0;
                    rB = 0;
                    valC = 0;
                    valP = 0;
                    Ins = "bubble";
                }
                else
                {
                    icode = f.icode;
                    ifun = f.ifun;
                    rA = f.rA;
                    rB = f.rB;
                    valC = f.valC;
                    valP = f.valP;
                    Ins = f.Ins;
                }
            }

        }

        public void output(StreamWriter writer)
        {
            writer.WriteLine("DECODE:");
            writer.WriteLine("\tD_icode \t= " + icode);
            writer.WriteLine("\tD_ifun \t\t= " + ifun);
            writer.WriteLine("\tD_rA \t\t= " + rA);
            writer.WriteLine("\tD_rB \t\t= " + rB);
            writer.WriteLine("\tD_valC \t\t= 0x" + valC.ToString("x"));
            writer.WriteLine("\tD_valP	\t= 0x" + valP.ToString("x"));
            writer.WriteLine();
        }


    }


    public class WritebackRegister
    {

        public int icode;
        public int valE;
        public int valM;
        public int dstE;
        public int dstM;
        public String Ins;

        public void init(MemoryStage m)
        {
            icode = m.icode;
            valE = m.valE;
            valM = m.valM;
            dstE = m.dstE;
            dstM = m.dstM;
            Ins = m.Ins;

        }

        public void output(StreamWriter writer)
        {
            writer.WriteLine("WRITE BACK:");
            writer.WriteLine("\tW_icode \t= " + icode);
            writer.WriteLine("\tW_valE \t\t= 0x" + valE.ToString("x"));
            writer.WriteLine("\tW_valM \t\t= 0x" + valM.ToString("x"));
            writer.WriteLine("\tW_dstE	\t= " + dstE);
            writer.WriteLine("\tW_dstM	\t= " + dstM);
            writer.WriteLine();

        }

        public bool Halt()
        {
            return icode == ConstVar.IHALT;
        }

        public bool Instrvalid()
        {
            if (icode < 0 || icode > 11) return true;
            return false;
        }
    }
}