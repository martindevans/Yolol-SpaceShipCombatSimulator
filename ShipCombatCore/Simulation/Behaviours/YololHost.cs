using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Myre.Entities;
using Myre.Entities.Behaviours;
using Yolol.Execution;
using Yolol.Grammar.AST;
using Yolol.IL;
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
            private readonly CompiledProgram _compiled;

            public Value[] Internals { get; }

            private CompiledProgramState(CompiledProgram compiled)
            {
                _compiled = compiled;

                Internals = new Value[compiled.InternalsMap.Count];
                Array.Fill(Internals, Number.Zero);
            }

            public void Tick(Value[] externals)
            {
                _compiled.Tick(Internals, externals);
            }

            public static CompiledProgramState Compile(Program program, ExternalsMap externalsMap)
            {
                return new CompiledProgramState(program.Compile(externalsMap, program.Lines.Count));
            }
        }

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly ExternalsMap _externalsMap;
        private readonly Value[] _externals;

        private readonly Dictionary<string, IVariable> _cache = new();

        private readonly List<CompiledProgramState> _compiled;

        public YololContext(IEnumerable<Program> programs)
        {
            _externalsMap = new ExternalsMap();

            var compiled = new List<CompiledProgramState>();

            foreach (var prog in programs.Where(p => p.Lines.Count > 0))
                compiled.Add(CompiledProgramState.Compile(prog, _externalsMap));
            _compiled = compiled;

            _externals = new Value[_externalsMap.Count];
            Array.Fill(_externals, (Number)0);

            Constants.SetConstants(this);
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

        public IVariable Get(string name)
        {
            if (!name.StartsWith(":"))
                throw new ArgumentException($"`{name}` is not an external variable");

            name = name.ToLowerInvariant();

            if (!_cache.TryGetValue(name, out var v))
            {
                v = ConstructVariable(name);
                _cache[name] = v;
            }
            return v;
        }

        private IVariable ConstructVariable(string name)
        {
            return _externalsMap.TryGetValue(name, out var idx)
                  ? new ArrayYololVariable(_externals, idx)
                  : new StandaloneYololVariable(name);
        }

        private class ArrayYololVariable
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

            public ArrayYololVariable(Value[] values, int index)
            {
                _values = values;
                _index = index;
            }
        }

        private class StandaloneYololVariable
            : IVariable
        {
            [UsedImplicitly] private readonly string _name;

            public Value Value { get; set; }

            public StandaloneYololVariable(string name)
            {
                _name = name;
                Value = Number.Zero;
            }
        }
    }

    [DefaultManager(typeof(Manager))]
    public class YololHost
        : ProcessBehaviour
    {
        private Property<YololContext>? _context;
        private Property<string>? _name;

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

            _context?.Value?.Tick();
        }

        public class Manager
            : Manager<YololHost>
        {
        }
    }
}
