using System;

namespace SampleWordHelper.Core
{
    public class SuspendFlag
    {
        bool flag;

        class SuspendHolder : IDisposable
        {
            readonly SuspendFlag parent;

            public SuspendHolder(SuspendFlag parent)
            {
                this.parent = parent;
                parent.flag = true;
            }

            public void Dispose()
            {
                parent.flag = false;
            }
        }

        public IDisposable Suspend()
        {
            return new SuspendHolder(this);
        }

        public static implicit operator bool(SuspendFlag flag)
        {
            return flag.flag;
        }
    }
}