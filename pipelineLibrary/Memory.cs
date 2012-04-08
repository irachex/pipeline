using System;
using System.Collections.Generic;
using System.Text;

namespace pipelineLibrary
{
    public class Memory
    {

        public byte[] Data;
        public int size;

        public Memory(int size)
        {
            Data = new byte[size];
            this.size = size;
        }
        public int splitHigh(int addr)
        {
            if (addr < 0 || addr >= size) return 1;
            int b = Data[addr];
            if (b < 0) b += 256;
            return b / 16;
        }

        public int splitLow(int addr)
        {
            if (addr < 0 || addr >= size) return 0;
            int b = Data[addr];
            if (b < 0) b += 256;
            return b % 16;
        }

        public int read(int addr)
        {
            if (addr < 0 || addr + 3 >= size)
                throw new Exception("Address Error");
            int x = 0;
            for (int i = 0; i < 4; i++)
                if (Data[addr + i] >= 0 || i == 0)
                    x = x * 256 + Data[addr + i];
                else
                    x = x * 256 + Data[addr + i] + 256;
            return x;
        }

        public void write(int addr, int val)
        {
            if (addr < 0 || addr + 3 >= size)
                throw new Exception("Address Error");
            for (int i = 3; i >= 0; i--)
            {
                int b = val % 256;
                if (b >= 128)
                    b -= 256;
                Data[addr + i] = (byte)b;
                val /= 256;
            }
        }

    }

}
