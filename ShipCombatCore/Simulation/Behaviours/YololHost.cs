using System;
using System.Collections.Generic;
using Myre.Entities;
using Myre.Entities.Behaviours;
using Yolol.Execution;
using Yolol.Grammar;
using Yolol.Grammar.AST;
using Yolol.IL.Compiler;
using Yolol.IL.Extensions;

namespace ShipCombatCore.Simulation.Behaviours
{
    public class YololContext
    {
        private class CompiledProgramState
        {
            public int ProgramCounter { get; private set; }
            public IReadOnlyList<Func<ArraySegment<Value>, ArraySegment<Value>, int>> Lines { get; }

            public InternalsMap InternalsMap { get; }
            private readonly Value[] _internals;

            private CompiledProgramState(IReadOnlyList<Func<ArraySegment<Value>, ArraySegment<Value>, int>> lines, InternalsMap internals)
            {
                Lines = lines;
                ProgramCounter = 0;

                InternalsMap = internals;
                _internals = new Value[InternalsMap.Count];
                Array.Fill(_internals, (Number)0);
            }

            public void Tick(Value[] externals)
            {
                if (Lines.Count == 0)
                    return;
                if (ProgramCounter >= Lines.Count)
                    ProgramCounter = 0;
                if (ProgramCounter < 0)
                    ProgramCounter = 0;
                ProgramCounter = Lines[ProgramCounter](_internals, externals) - 1;
            }

            public static CompiledProgramState Compile(Program program, ExternalsMap externalsMap)
            {
                var internals = new InternalsMap();
                return new CompiledProgramState(program.Compile(internals, externalsMap, program.Lines.Count), internals);
            }
        }

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly ExternalsMap _externalsMap;
        private readonly Value[] _externals;

        private readonly List<CompiledProgramState> _compiled;

        public YololContext(IEnumerable<Program> programs)
        {
            _externalsMap = new ExternalsMap();

            var compiled = new List<CompiledProgramState>();
            foreach (var prog in programs)
                compiled.Add(CompiledProgramState.Compile(prog, _externalsMap));
            _compiled = compiled;

            _externals = new Value[_externalsMap.Count];
            Array.Fill(_externals, (Number)0);
        }

        public void Tick()
        {
            foreach (var state in _compiled)
                state.Tick(_externals);
        }

        public YololVariable Get(string name)
        {
            return MaybeGet(name) ?? new YololVariable(_externals, -1);
        }

        public YololVariable? MaybeGet(string name)
        {
            if (!name.StartsWith(":"))
                throw new ArgumentException($"`{name}` is not an external variable");

            if (!_externalsMap.TryGetValue(name, out var idx))
                return null;
            return new YololVariable(_externals, idx);
        }
    }

    public class YololVariable
        : IVariable
    {
        private readonly Value[] _values;
        private readonly int _index;

        public Value Value
        {
            get
            {
                if (_index == -1)
                    return new Value(false);
                else
                    return _values[_index];
            }
            set
            {
                if (_index == -1)
                    return;
                _values[_index] = value;
            }
        }

        public YololVariable(Value[] values, int index)
        {
            _values = values;
            _index = index;
        }
    }

    [DefaultManager(typeof(Manager))]
    public class YololHost
        : ProcessBehaviour
    {
        private Property<YololContext>? _context;

        private double _accumulatedTime;

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _context = context.CreateProperty(PropertyNames.YololContext);

            base.CreateProperties(context);
        }

        protected override void Update(float elapsedTime)
        {
            _accumulatedTime += elapsedTime * 1000;
            while (_accumulatedTime > 1)
            {
                _accumulatedTime--;
                _context?.Value?.Tick();
            }
        }

        public class Manager
            : Manager<YololHost>
        {
        }
    }
}
