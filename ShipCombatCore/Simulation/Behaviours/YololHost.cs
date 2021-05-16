using System;
using System.Collections.Generic;
using Myre.Entities;
using Myre.Entities.Behaviours;
using Yolol.Execution;
using Yolol.Grammar.AST;
using Yolol.IL.Compiler;
using Yolol.IL.Extensions;

namespace ShipCombatCore.Simulation.Behaviours
{
    public class YololContext
    {
        private const int MaxStringLength = 10000;
        private const int MaxMemoryUsage = 1000000;

        private class CompiledProgramState
        {
            public int ProgramCounter { get; private set; }
            public IReadOnlyList<Func<ArraySegment<Value>, ArraySegment<Value>, int>> Lines { get; }

            public InternalsMap InternalsMap { get; }
            public Value[] Internals { get; }

            private CompiledProgramState(IReadOnlyList<Func<ArraySegment<Value>, ArraySegment<Value>, int>> lines, InternalsMap internals)
            {
                Lines = lines;
                ProgramCounter = 0;

                InternalsMap = internals;
                Internals = new Value[InternalsMap.Count];
                Array.Fill(Internals, (Number)0);
            }

            public void Tick(Value[] externals)
            {
                if (Lines.Count == 0)
                    return;
                if (ProgramCounter >= Lines.Count)
                    ProgramCounter = 0;
                if (ProgramCounter < 0)
                    ProgramCounter = 0;
                ProgramCounter = Lines[ProgramCounter](Internals, externals) - 1;
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

            // Check memory usage of externals. Trim strings which are over the max length limit
            var memory = 0;
            for (var i = 0; i < _externals.Length; i++)
                memory += CheckMemory(ref _externals[i]);

            // Check memory usage of internals of each chip. Trim string which are over the limit.
            var maxMemChipIdx = -1;
            var maxMemChipUsage = 0;
            for (var c = 0; c < _compiled.Count; c++)
            {
                var m = 0;
                for (var i = 0; i < _compiled[c].Internals.Length; i++)
                    m += CheckMemory(ref _compiled[c].Internals[i]);
                memory += m;
                if (m > maxMemChipUsage)
                {
                    maxMemChipIdx = c;
                    maxMemChipUsage = m;
                }
            }

            // If total ship memory usage is over the limit delete the chip using the most memory
            if (memory > MaxMemoryUsage && maxMemChipIdx >= 0)
                _compiled.RemoveAt(maxMemChipIdx);
        }

        private static int CheckMemory(ref Value value)
        {
            if (value.Type == Yolol.Execution.Type.String)
            {
                if (value.String.Length > MaxStringLength)
                {
                    value = "";
                    return 0;
                }
                else
                    return value.String.Length;
            }
            else
                return 1;
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
        private Property<string>? _name;

        private double _accumulatedTime;
        
        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _context = context.CreateProperty(PropertyNames.YololContext);
            _name = context.CreateProperty(PropertyNames.UniqueName);

            base.CreateProperties(context);
        }

        protected override void Update(float elapsedTime)
        {
            var name = _context?.Value?.Get(":name");
            if (name != null && _name?.Value != null)
                name.Value = _name.Value;

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
