﻿using System.Collections.Generic;
using Myre.Collections;
using Myre.Entities;
using Myre.Entities.Behaviours;
using ShipCombatCore.Extensions;
using Yolol.Execution;

namespace ShipCombatCore.Simulation.Behaviours
{
    [DefaultManager(typeof(Manager))]
    public class Radio
        : ProcessBehaviour
    {
#pragma warning disable 8618
        private Property<YololContext> _context;
        private Property<uint> _team;
        private Manager _manager;
#pragma warning restore 8618

        private IVariable? _send;
        private IVariable? _recv;
        

        public override void CreateProperties(Entity.ConstructionContext context)
        {
            _context = context.CreateProperty(PropertyNames.YololContext);
            _team = context.CreateProperty(PropertyNames.TeamOwner);

            base.CreateProperties(context);
        }

        public override void Initialise(INamedDataProvider? initialisationData)
        {
            _manager = Owner.Scene!.GetManager<Manager>();

            base.Initialise(initialisationData);
        }

        protected override void Update(float elapsedTime)
        {
            var ctx = _context.Value;   
            if (ctx == null)
                return;

            _send ??= ctx.Get(":radio_tx");
            _recv ??= ctx.Get(":radio_rx");

            if (_send.Value.Type == Type.String && _send.Value.String.Length > 0)
                _manager.Send(_team.Value, _send.Value.ToString());
            _send.Value = "";

            if (_manager.Receive(_team.Value, out var received))
                _recv.Value = received;
            else
                _recv.Value = Number.Zero;
        }

        private class Manager
            : Manager<Radio>
        {
            private readonly Dictionary<uint, Queue<string>> _sent = new();
            private readonly Dictionary<uint, string> _received = new();

            public void Send(uint team, string message)
            {
                _sent.GetOrAdd(team, _ => new Queue<string>()).Enqueue(message);
            }

            public bool Receive(uint team, out string output)
            {
                return _received.TryGetValue(team, out output);
            }

            protected override void Update(float elapsedTime)
            {
                base.Update(elapsedTime);

                _received.Clear();

                foreach (var (team, messages) in _sent)
                {
                    if (messages.Count == 0)
                        continue;

                    _received[team] = messages.Dequeue();
                }

                _sent.Clear();
            }
        }
    }
}
