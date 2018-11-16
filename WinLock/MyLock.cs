using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinLock
{
    internal class MyLock
    {
        private List<char> input_stream = new List<char>();
        private char[] codeArr;

        public MyLock(string code)
        {
            this.codeArr = code.ToCharArray();
        }
        internal delegate void UnlockDelegate();

        public event UnlockDelegate Unlock;

        public int Check(char code)
        {
            int num = 0;
            this.input_stream.Add(code);
            if (this.input_stream.Count > this.codeArr.Length)
                this.input_stream.RemoveAt(0);
            if (this.input_stream.Count == this.codeArr.Length)
            {
                for (int index = this.input_stream.Count - 1; index > 0; --index)
                {
                    if ((int)this.input_stream[index] != (int)this.codeArr[index])
                    {
                        num = -1;
                        break;
                    }
                }
                if (num == 0)
                {
                    // ISSUE: reference to a compiler-generated field
                    UnlockDelegate unlock = this.Unlock;
                    if (unlock != null)
                        unlock();
                }
            }
            return num;
        }
    }
}