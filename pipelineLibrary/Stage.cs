using System;
using System.Collections.Generic;
using System.Text;

namespace pipelineLibrary
{
    public class FetchStage
    {
        public int pc;
        public int icode;
        public int ifun;
        public int rA;
        public int rB;
        public int valC;
        public int valP;
        public int predPC;
        public FetchRegister F;
        public Memory Mem;
        public String Ins;

        public void translateIns()
        {
            try
            {
                if (icode == 6)
                    Ins = ConstVar.OPl[ifun];
                else
                    if (icode == 7)
                        Ins = ConstVar.JXX[ifun];
                    else
                        Ins = ConstVar.Ins[icode];
                if (icode == 0 || icode == 1 || icode == 9) return;
                if (icode == 10 || icode == 11 || icode == 2 || icode == 6)
                {
                    Ins = Ins + " " + ConstVar.Reg[rA];
                    if (rB != 8)
                        Ins = Ins + ", " + ConstVar.Reg[rB];
                    return;
                }
                if (icode == 7 || icode == 8)
                {
                    Ins = Ins + " 0x" + valC.ToString("x");
                    return;
                }
                if (icode == 3)
                    Ins = Ins + " 0x" + valC.ToString("x") + ", " + ConstVar.Reg[rB];
                else
                    if (icode == 4)
                        Ins = Ins + " " + ConstVar.Reg[rA] + ", 0x"
                                + valC.ToString("x") + "(" + ConstVar.Reg[rB] + ")";
                    else
                        if (icode == 5)
                            Ins = Ins + " 0x" + valC.ToString("x") +
                                    "(" + ConstVar.Reg[rB] + "), " + ConstVar.Reg[rA];
            }
            catch (Exception e)
            {
                Ins = "Invalid insturction";
            }
        }

        public void SelectPC(MemoryRegister M, WritebackRegister W)
        {
            if (M.icode == ConstVar.IJXX && !M.Bch)
                pc = M.valA;
            else
                if (W.icode == ConstVar.IRET)
                    pc = W.valM;
                else
                    pc = F.predPC;
        }

        void Split()
        {
            icode = Mem.splitHigh(pc);
            ifun = Mem.splitLow(pc);
        }

        bool Needregids()
        {
            int[] Set = new int[]{ConstVar.IRRMOVL,ConstVar.IIRMOVL,ConstVar.IRMMOVL,ConstVar.IMRMOVL,ConstVar.IOPL,
				ConstVar.IPUSHL,ConstVar.IPOPL};
            for (int i = 0; i < 7; i++)
                if (icode == Set[i])
                    return true;
            return false;
        }

        void Align()
        {
            int fix = 0;
            if (Needregids())
            {
                rA = Mem.splitHigh(pc + 1);
                rB = Mem.splitLow(pc + 1);
                fix = 1;
            }
            else
            {
                rA = 8;
                rB = 8;
            }
            valC = 0;
            if (NeedvalC())
            {
                for (int i = 0; i < 4; i++)
                {
                    valC = valC * 16 + Mem.splitHigh(pc + 1 + fix + i);
                    valC = valC * 16 + Mem.splitLow(pc + 1 + fix + i);
                }
            }
        }

        bool NeedvalC()
        {
            int[] Set = new int[] { ConstVar.IIRMOVL, ConstVar.IRMMOVL, ConstVar.IMRMOVL, ConstVar.IJXX, ConstVar.ICALL };
            for (int i = 0; i < 5; i++)
                if (icode == Set[i])
                    return true;
            return false;
        }

        void PCincrement()
        {
            valP = pc + 1;
            if (Needregids())
                valP = valP + 1;
            if (NeedvalC())
                valP = valP + 4;
        }

        void PredictPC()
        {
            if (icode == ConstVar.IJXX || icode == ConstVar.ICALL)
                predPC = valC;
            else
                predPC = valP;
        }

        public void executeStage()
        {
            Split();
            Align();
            PCincrement();
            PredictPC();
        }
    }


    public class DecodeStage
    {

        public int dstE;
        public int dstM;
        public int srcA;
        public int srcB;
        public int valA;
        public int valB;
        public int valC;
        public int icode;
        public int ifun;
        public DecodeRegister D;
        public Registerfile rf;
        public String Ins;

        void setdstE()
        {
            int[] Set1 = new int[] { ConstVar.IRRMOVL, ConstVar.IIRMOVL, ConstVar.IOPL };
            int[] Set2 = new int[] { ConstVar.IPUSHL, ConstVar.IPOPL, ConstVar.IRET, ConstVar.ICALL };
            for (int i = 0; i < 3; i++)
                if (icode == Set1[i])
                {
                    dstE = D.rB;
                    return;
                }
            for (int i = 0; i < 4; i++)
                if (icode == Set2[i])
                {
                    dstE = ConstVar.RESP;
                    return;
                }
            dstE = ConstVar.RNONE;
        }

        void setdstM()
        {
            if (icode == ConstVar.IPOPL || icode == ConstVar.IMRMOVL)
                dstM = D.rA;
            else
                dstM = ConstVar.RNONE;
        }

        void setsrcA()
        {
            int[] Set1 = new int[] { ConstVar.IRRMOVL, ConstVar.IRMMOVL, ConstVar.IOPL, ConstVar.IPUSHL };
            int[] Set2 = new int[] { ConstVar.IPOPL, ConstVar.IRET };
            for (int i = 0; i < 4; i++)
                if (icode == Set1[i])
                {
                    srcA = D.rA;
                    return;
                }
            for (int i = 0; i < 2; i++)
                if (icode == Set2[i])
                {
                    srcA = ConstVar.RESP;
                    return;
                }
            srcA = ConstVar.RNONE;
        }

        void setsrcB()
        {
            int[] Set1 = new int[] { ConstVar.IRMMOVL, ConstVar.IMRMOVL, ConstVar.IOPL };
            int[] Set2 = new int[] { ConstVar.IPOPL, ConstVar.IRET, ConstVar.IPUSHL, ConstVar.ICALL };
            for (int i = 0; i < 3; i++)
                if (icode == Set1[i])
                {
                    srcB = D.rB;
                    return;
                }
            for (int i = 0; i < 4; i++)
                if (icode == Set2[i])
                {
                    srcB = ConstVar.RESP;
                    return;
                }
            srcB = ConstVar.RNONE;
        }

        public void SelFwdA(ExecuteStage e, ExecuteRegister E, MemoryStage m, MemoryRegister M, WritebackRegister W)
        {
            if (icode == ConstVar.IJXX || icode == ConstVar.ICALL)
                valA = D.valP;
            else
            {
                int[] Set = new int[] { E.dstE, M.dstM, M.dstE, W.dstM, W.dstE };
                int[] Val = new int[] { e.valE, m.valM, M.valE, W.valM, W.valE };
                for (int i = 0; i < 5; i++)
                    if (Set[i] == srcA)
                    {
                        valA = Val[i];
                        return;
                    }
                valA = rf.Read(srcA);
            }

        }

        public void FwdB(ExecuteStage e, ExecuteRegister E, MemoryStage m, MemoryRegister M, WritebackRegister W)
        {
            int[] Set = new int[] { E.dstE, M.dstM, M.dstE, W.dstM, W.dstE };
            int[] Val = new int[] { e.valE, m.valM, M.valE, W.valM, W.valE };
            for (int i = 0; i < 5; i++)
                if (Set[i] == srcB)
                {
                    valB = Val[i];
                    return;
                }
            valB = rf.Read(srcB);
        }

        public void writeBack(WritebackRegister W)
        {
            rf.write(W.dstE, W.valE);
            rf.write(W.dstM, W.valM);
        }

        public void executeStage()
        {
            icode = D.icode;
            ifun = D.ifun;
            valC = D.valC;
            setdstE();
            setdstM();
            setsrcA();
            setsrcB();
            Ins = D.Ins;
        }
    }

    public class MemoryStage
    {

        public int valM;
        public int valE;
        public int dstE;
        public int dstM;
        public int icode;
        public MemoryRegister M;
        public Memory Mem;
        public String Ins;

        bool Memread()
        {
            return M.icode == ConstVar.IMRMOVL || M.icode == ConstVar.IPOPL || M.icode == ConstVar.IRET;
        }

        bool Memwrite()
        {
            return M.icode == ConstVar.IRMMOVL || M.icode == ConstVar.IPUSHL || M.icode == ConstVar.ICALL;
        }

        int Addr()
        {
            int[] Set1 = new int[] { ConstVar.IRMMOVL, ConstVar.IPUSHL, ConstVar.ICALL, ConstVar.IMRMOVL };
            for (int i = 0; i < 4; i++)
                if (M.icode == Set1[i])
                    return M.valE;
            if (M.icode == ConstVar.IPOPL || M.icode == ConstVar.IRET)
                return M.valA;
            return 0;
        }

        void setvalM()
        {
            valM = 0;
            if (Memread())
                valM = Mem.read(Addr());
        }

        public void writeMemory()
        {
            if (Memwrite())
                Mem.write(Addr(), M.valA);
        }

        public void executeStage()
        {

            icode = M.icode;
            valE = M.valE;
            setvalM();
            dstE = M.dstE;
            dstM = M.dstM;
            Ins = M.Ins;

        }
    }

    public class ExecuteStage
    {

        public int valE;
        public int valA;
        public int dstE;
        public int dstM;
        public int icode;
        public bool Bch;
        bool ZF, OF, SF;
        public ExecuteRegister E;
        public ControlCode CC;
        public String Ins;

        bool SetresetCC()
        {
            return E.icode == ConstVar.IOPL && E.ifun < 4 && E.ifun >= 0;
        }

        int ALUA()
        {
            if (E.icode == ConstVar.IRRMOVL || E.icode == ConstVar.IOPL)
                return E.valA;
            if (E.icode == ConstVar.IIRMOVL || E.icode == ConstVar.IRMMOVL || E.icode == ConstVar.IMRMOVL)
                return E.valC;
            if (E.icode == ConstVar.ICALL || E.icode == ConstVar.IPUSHL)
                return -4;
            if (E.icode == ConstVar.IRET || E.icode == ConstVar.IPOPL)
                return 4;
            return 0;
        }

        int ALUB()
        {
            int[] Set = new int[]{ConstVar.IRMMOVL,ConstVar.IMRMOVL,ConstVar.IOPL,ConstVar.ICALL,ConstVar.IPUSHL,
				ConstVar.IRET,ConstVar.IPOPL};
            for (int i = 0; i < 7; i++)
                if (E.icode == Set[i])
                    return E.valB;
            return 0;
        }

        int ALUfun()
        {
            if (E.icode == ConstVar.IOPL)
                return E.ifun;
            return ConstVar.ALUADD;
        }

        void ALU()
        {
            int a = ALUA(), b = ALUB();
            switch (ALUfun())
            {
                case 0:
                    valE = a + b;
                    OF = ((a < 0 == b < 0) && (valE < 0 != a < 0));
                    SF = (valE < 0);
                    ZF = (valE == 0);
                    break;
                case 1:
                    a = -a;
                    valE = a + b;
                    OF = ((a < 0 == b < 0) && (valE < 0 != a < 0));
                    SF = (valE < 0);
                    ZF = (valE == 0);
                    break;
                case 2:
                    valE = a & b;
                    OF = false;
                    SF = (valE < 0);
                    ZF = (valE == 0);
                    break;
                case 3:
                    valE = a ^ b;
                    OF = false;
                    SF = (valE < 0);
                    ZF = (valE == 0);
                    break;
                default:
                    valE = 0;
                    OF = false;
                    SF = false;
                    ZF = false;
                    break;
            }
        }

        void resetCC()
        {
            if (SetresetCC())
            {
                CC.setOF(OF);
                CC.setSF(SF);
                CC.setZF(ZF);
            }
        }

        void bcond()
        {
            if (E.icode == ConstVar.IJXX)
            {
                switch (E.ifun)
                {
                    case 0:
                        Bch = true;
                        break;
                    case 1:
                        Bch = (OF ^ SF) || ZF;
                        break;
                    case 2:
                        Bch = (OF ^ SF);
                        break;
                    case 3:
                        Bch = ZF;
                        break;
                    case 4:
                        Bch = !ZF;
                        break;
                    case 5:
                        Bch = !(OF ^ SF);
                        break;
                    case 6:
                        Bch = !(OF ^ SF) && !ZF;
                        break;
                    default:
                        Bch = false;
                        break;
                }
            }
        }

        public void executeStage()
        {
            icode = E.icode;
            ALU();
            resetCC();
            bcond();
            valA = E.valA;
            dstE = E.dstE;
            dstM = E.dstM;
            Ins = E.Ins;
        }
    }

}
