//     Copyright (c) Microsoft Corporation.  All rights reserved.

using Microsoft.Win32.SafeHandles;

namespace SampleWordHelper.Core.IO {
    internal sealed class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid {
        internal SafeFindHandle()
            : base(true) {
        }

        protected override bool ReleaseHandle() {
            return NativeMethods.FindClose(base.handle);
        }
    }
}
