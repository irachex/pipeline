using System;
using System.Collections.Generic;
using System.Text;

namespace pipelineLibrary
{
    public static class ConstVar
    {

        public static int INOP = 0;
        public static int IHALT = 1;
        public static int IRRMOVL = 2;
        public static int IIRMOVL = 3;
        public static int IRMMOVL = 4;
        public static int IMRMOVL = 5;
        public static int IOPL = 6;
        public static int IJXX = 7;
        public static int ICALL = 8;
        public static int IRET = 9;
        public static int IPUSHL = 10;
        public static int IPOPL = 11;

        public static int RESP = 4;
        public static int RNONE = 8;

        public static int ALUADD = 0;
        public static string[] Reg = new string[] { "%eax", "%ecx", "%edx", "%ebx", "%esp", "%ebp", "%esi", "%edi" };

        public static string[] Ins = new string[]{"nop","halt","rrmovl","irmovl","rmmovl","mrmovl","OPl",
			"JXX","call","ret","pushl","popl"};
        public static string[] OPl = new string[] { "addl", "subl", "andl", "xorl" };
        public static string[] JXX = new string[] { "jmp", "jle", "jl", "je", "jne", "jge", "jg" };

        public static String ConvertToReg(int i)
        {

            if (i == 8) return "";
            else return Reg[i];
        }

    }

    public class Registerfile
    {

        public int[] Data = new int[8];

        public void write(int src, int val)
        {
            if (0 <= src && src < 8)
                Data[src] = val;
        }

        public int Read(int src)
        {
            if (src >= 8 || src < 0) return 0;
            return Data[src];
        }

    }


    public class ControlCode
    {

        public bool OF;
        public bool ZF;
        public bool SF;

        public void setOF(bool oF)
        {
            OF = oF;
        }

        public bool getOF()
        {
            return OF;
        }

        public void setSF(bool sF)
        {
            SF = sF;
        }

        public bool getSF()
        {
            return SF;
        }

        public void setZF(bool zF)
        {
            ZF = zF;
        }

        public bool getZF()
        {
            return ZF;
        }

    }

    public class ControlLogic
    {

        ExecuteRegister E;
        DecodeStage d;
        DecodeRegister D;
        MemoryRegister M;
        ExecuteStage e;

        public void set(DecodeRegister D, DecodeStage d, ExecuteRegister E, ExecuteStage e, MemoryRegister M)
        {
            this.D = D;
            this.E = E;
            this.M = M;
            this.d = d;
            this.e = e;
        }

        public bool detectLoadUse()
        {
            return (E.icode == ConstVar.IMRMOVL || E.icode == ConstVar.IPOPL)
                    && (E.dstM == d.srcA || E.dstM == d.srcB);
        }

        public bool detectRet()
        {
            return ConstVar.IRET == D.icode || ConstVar.IRET == E.icode || ConstVar.IRET == M.icode;
        }

        public bool detectMispred()
        {
            return E.icode == ConstVar.IJXX && !e.Bch;
        }

        public bool Fstall()
        {
            return detectLoadUse() || detectRet();
        }

        public bool Dstall()
        {
            return detectLoadUse();
        }

        public bool Dbubble()
        {
            return detectMispred() || detectRet();
        }

        public bool Ebubble()
        {
            return detectMispred() || detectLoadUse();
        }

    }

}
