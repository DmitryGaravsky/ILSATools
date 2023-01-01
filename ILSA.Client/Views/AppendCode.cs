namespace ILSA.Client.Views {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;
    using System.Text;
    using DevExpress.XtraEditors;
    using ILReader.Readers;

    static class CodeBoxExtension {
        public static void AppendLines(this MemoEdit codeBox, IEnumerable<IInstruction> instructions) {
            StringBuilder code = new StringBuilder(1024);
            AppendLines(code, instructions);
            codeBox.Clear();
            codeBox.AppendText(code.ToString());
            codeBox.Select(0, 0);
            codeBox.ScrollToCaret();
            codeBox.Refresh();
        }
        static void AppendLines(StringBuilder code, IEnumerable<IInstruction> instructions) {
            var reader = instructions as IILReader;
            if(reader != null && reader.Metadata.Any()) {
                AppendLines(code, reader.Metadata);
                code.AppendLine();
            }
            Dictionary<int, string> exceptionBlocks = new Dictionary<int, string>();
            if(reader != null) {
                for(int i = 0; i < reader.ExceptionHandlers.Length; i++) {
                    var eh = reader.ExceptionHandlers[i];
                    PrepareExceptionBlockLine(exceptionBlocks, eh.TryStart.Index, ".try {", i);
                    PrepareExceptionBlockLine(exceptionBlocks, eh.TryEnd.Index, "}  // end .try", i);
                    if(eh.IsFinally)
                        PrepareExceptionBlockLine(exceptionBlocks, eh.HandlerStart.Index, "finally {", i);
                    if(eh.IsFault)
                        PrepareExceptionBlockLine(exceptionBlocks, eh.HandlerStart.Index, "fault {", i);
                    if(eh.IsCatch)
                        PrepareExceptionBlockLine(exceptionBlocks, eh.HandlerStart.Index, "catch " + eh.CatchType + " {", i);
                    if(eh.IsFilter)
                        PrepareExceptionBlockLine(exceptionBlocks, eh.FilterStart.Index, "filter {", i);
                    PrepareExceptionBlockLine(exceptionBlocks, eh.HandlerEnd.Index, "} // end handler", i);
                }
            }
            if(instructions != null) {
                foreach(var instruction in instructions) {
                    string ebLines;
                    if(exceptionBlocks.TryGetValue(instruction.Index, out ebLines))
                        AppendExceptionBlockLines(code, ebLines, instruction.Depth - 1);
                    if(instruction.OpCode == OpCodes.Ldstr)
                        AppendLdSrtLine(code, instruction);
                    else
                        AppendLine(code, instruction);
                }
            }
        }
        static readonly string[] splitLines = new string[] { Environment.NewLine };
        static void AppendExceptionBlockLines(StringBuilder code, string exceptionBlock, int depth) {
            var lines = exceptionBlock.Split(splitLines, StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0; i < lines.Length; i++) {
                code.Append(lines[i], 0, 6);
                if(depth > 0)
                    code.Append(' ', depth * 2);
                code.Append(lines[i], 6, lines[i].Length - 6);
                code.AppendLine();
            }
        }
        static void PrepareExceptionBlockLine(Dictionary<int, string> exceptionBlocks, int index, string text, int i) {
            var blockLine = ("@" + i.ToString("X2")).PadRight(8) + text + Environment.NewLine;
            string line;
            if(!exceptionBlocks.TryGetValue(index, out line))
                exceptionBlocks.Add(index, blockLine);
            else
                exceptionBlocks[index] = line + blockLine;
        }
        static void AppendLines(StringBuilder code, IEnumerable<IMetadataItem> metadata, int offset = 0) {
            foreach(var meta in metadata) {
                if(meta.HasChildren) {
                    AppendLine(code, meta.Name, "(" + Environment.NewLine, offset, false);
                    AppendLines(code, meta.Children, offset + 4);
                    AppendLine(code, null, ")", offset);
                }
                else AppendLine(code, meta.Name, meta.Value, offset);
            }
        }
        static void AppendLine(StringBuilder code, string meta, object value, int offset = 0, bool newLine = true) {
            if(meta != null) {
                var metaName = meta.PadLeft(offset + meta.Length);
                code.Append(metaName);
            }
            if(value != null) {
                var metaValue = meta != null ? " " + value.ToString() : value.ToString();
                code.Append(metaValue);
            }
            if(newLine) code.AppendLine();
        }
        static void AppendLdSrtLine(StringBuilder code, IInstruction instruction) {
            string line = instruction.Text;
            code.Append(line, 0, line.IndexOf(':') + 2);
            if(instruction.Depth > 0)
                code.Append(' ', instruction.Depth * 2);
            code.Append(nameof(OpCodes.Ldstr));
            string value = (string)instruction.Operand;
            code.Append(" \"").Append(value).Append("\"");
            code.AppendLine();
        }
        static void AppendLine(StringBuilder code, IInstruction instruction) {
            string line = instruction.Text;
            int offsetIndex = line.IndexOf(':') + 2;
            code.Append(line, 0, offsetIndex);
            if(instruction.Depth > 0)
                code.Append(' ', instruction.Depth * 2);
            code.Append(line, offsetIndex, line.Length - offsetIndex);
            code.AppendLine();
        }
    }
}