﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Razor.Generator.Compiler
{
    public struct CSharpCodeWritingScope : IDisposable
    {
        private CodeWriter _writer;
        private bool _autoSpace;
        private int _tabSize;
        private int _startIndent;

        public CSharpCodeWritingScope(CodeWriter writer) : this(writer, true) { }
        public CSharpCodeWritingScope(CodeWriter writer, int tabSize) : this(writer, tabSize, true) { }
        // TODO: Make indents (tabs) environment specific
        public CSharpCodeWritingScope(CodeWriter writer, bool autoSpace) : this(writer, 4, autoSpace) { }
        public CSharpCodeWritingScope(CodeWriter writer, int tabSize, bool autoSpace)
        {
            _writer = writer;
            _autoSpace = true;
            _tabSize = tabSize;
            _startIndent = -1; // Set in WriteStartScope

            OnClose = () => { };

            WriteStartScope();
        }

        public event Action OnClose;

        public void Dispose()
        {
            WriteEndScope();
            OnClose();
        }

        private void WriteStartScope()
        {
            TryAutoSpace(" ");

            _writer.WriteLine("{").IncreaseIndent(_tabSize);
            _startIndent = _writer.CurrentIndent;
        }

        private void WriteEndScope()
        {
            TryAutoSpace(Environment.NewLine);

            // Ensure the scope hasn't been modified
            if (_writer.CurrentIndent == _startIndent)
            {
                _writer.DecreaseIndent(_tabSize);
            }

            _writer.WriteLine("}");
        }

        private void TryAutoSpace(string spaceCharacter)
        {
            if (_autoSpace && !Char.IsWhiteSpace(_writer.LastWrite.Last()))
            {
                _writer.Write(spaceCharacter);
            }
        }
    }
}