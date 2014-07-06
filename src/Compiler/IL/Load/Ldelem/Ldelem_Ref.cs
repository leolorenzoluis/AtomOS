﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atomix.IL;
using Atomix.Assembler;
using Atomix.Assembler.x86;
using Atomix.CompilerExt;
using System.Reflection;
using Atomix.ILOpCodes;
using Core = Atomix.Assembler.AssemblyHelper;

namespace Atomix.IL
{
    [ILOp(ILCode.Ldelem_Ref)]
    public class Ldelem_Ref : MSIL
    {
        public Ldelem_Ref(Compiler Cmp)
            : base("ldelemref", Cmp) { }

        public override void Execute(ILOpCode instr, MethodBase aMethod)
        {
            switch (ILCompiler.CPUArchitecture)
            {
                #region _x86_
                case CPUArch.x86:
                    {
                        Ldelem_x86(4, false);
                    }
                    break;
                #endregion
                #region _x64_
                case CPUArch.x64:
                    {

                    }
                    break;
                #endregion
                #region _ARM_
                case CPUArch.ARM:
                    {

                    }
                    break;
                #endregion
            }
        }

        public static void Ldelem_x86(int aElementSize, bool isSigned)
        {
            if (aElementSize <= 0 || aElementSize > 8 || (aElementSize > 4 && aElementSize < 8))
                throw new Exception("@Ldelem: Unsupported size for Ldelem_Ref: " + aElementSize);

            Core.vStack.Pop();
            Core.vStack.Pop();

            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EAX });
            Core.AssemblerCode.Add(new Mov { DestinationReg = Registers.EDX, SourceRef = "0x" + aElementSize.ToString("X") });
            Core.AssemblerCode.Add(new Multiply { DestinationReg = Registers.EDX });

            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EAX, SourceRef = "0x10" });

            Core.AssemblerCode.Add(new Pop { DestinationReg = Registers.EDX });
            Core.AssemblerCode.Add(new Add { DestinationReg = Registers.EDX, SourceReg = Registers.EAX });

            switch (aElementSize)
            {
                case 1:
                    if (isSigned)
                        Core.AssemblerCode.Add(new Movsx { DestinationReg = Registers.ECX, Size = 8, SourceReg = Registers.EDX, SourceIndirect = true });
                    else
                        Core.AssemblerCode.Add(new Movzx { DestinationReg = Registers.ECX, Size = 8, SourceReg = Registers.EDX, SourceIndirect = true });
                    Core.AssemblerCode.Add(new Push { DestinationReg = Registers.ECX });
                    break;
                case 2:
                    if (isSigned)
                        Core.AssemblerCode.Add(new Movsx { DestinationReg = Registers.ECX, Size = 16, SourceReg = Registers.EDX, SourceIndirect = true });
                    else
                        Core.AssemblerCode.Add(new Movzx { DestinationReg = Registers.ECX, Size = 16, SourceReg = Registers.EDX, SourceIndirect = true });
                    Core.AssemblerCode.Add(new Push { DestinationReg = Registers.ECX });
                    break;
                case 4:
                    Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EDX, DestinationIndirect = true });
                    break;
                case 8:
                    Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EDX, DestinationIndirect = true });
                    Core.AssemblerCode.Add(new Push { DestinationReg = Registers.EDX, DestinationDisplacement = 4, DestinationIndirect = true });
                    break;
            }

            if (aElementSize <= 4)
                Core.vStack.Push(aElementSize.Align(), isSigned ? typeof(int) : typeof(uint));
            else
                Core.vStack.Push(aElementSize.Align(), isSigned ? typeof(long) : typeof(ulong));
        }
    }
}