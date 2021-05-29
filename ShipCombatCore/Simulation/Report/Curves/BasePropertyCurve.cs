using System;
using System.Collections.Generic;
using Myre.Entities;
using Newtonsoft.Json;

namespace ShipCombatCore.Simulation.Report.Curves
{
    public interface ICurve
    {
        public void Serialize(JsonWriter writer);
    }

    public abstract class BasePropertyCurve<T>
        : BaseCurve<T>
        where T : struct
    {
        private readonly Property<T> _property;

        protected BasePropertyCurve(Property<T> property, uint optimisationWatermark = 100000)
            : base(property.Name, optimisationWatermark)
        {
            _property = property;
        }

        public void Extend(uint ms)
        {
            Extend(ms, _property.Value);
        }
    }

    public abstract class BaseCurve<T>
        : ICurve
        where T : struct
    {
        private readonly uint _minWatermark;
        private readonly string _name;
        private uint _optimisationWatermark;
        private readonly List<KeyFrame> _keyframes = new();

        protected BaseCurve(string name, uint optimisationWatermark = 100000)
        {
            _minWatermark = optimisationWatermark / 2;
            _name = name;
            _optimisationWatermark = optimisationWatermark;
        }

        private readonly struct KeyFrame
        {
            public readonly TimeSpan Time;
            public readonly T Value;

            public KeyFrame(TimeSpan time, T value)
            {
                Time = time;
                Value = value;
            }
        }

        public void Extend(uint ms, T value)
        {
            _keyframes.Add(new KeyFrame(TimeSpan.FromMilliseconds(ms), value));

            if (_keyframes.Count >= 3)
            {
                var ai = _keyframes.Count - 3;
                var bi = _keyframes.Count - 2;
                var ci = _keyframes.Count - 1;

                if (CanRemove(_keyframes[ai], _keyframes[bi], _keyframes[ci]))
                    _keyframes.RemoveAt(bi);
            }

            // Once a lot of keyframes have accumulated in memory optimise them with linear keyframe reduction to reduce memory usage.
            // Set the threshold for the next optimisation at 5x whatever this optimisation pass manages.
            if (_keyframes.Count > _optimisationWatermark)
            {
                KeyframeReduction(_keyframes);
                _optimisationWatermark = Math.Max(_minWatermark, (uint)(_keyframes.Count * 5));
            }
        }

        protected abstract T Estimate(in T start, in T end, float t);

        protected abstract float Error(in T expected, in T estimated);

        protected abstract void WriteKeyframeElements(JsonWriter writer, in T value);

        private void KeyframeReduction(List<KeyFrame> keyframes)
        {
            var ll = new LinkedList<KeyFrame>(keyframes);

            // Ported from the old Myre animation content processing pipeline:
            // https://github.com/martindevans/Myre/blob/45c2f9595d9167608e4d98795c8d0ff19d05a91c/Myre/Myre.Graphics.Pipeline/Animations/EmbeddedAnimationProcessor.cs#L273

            if (ll.First?.Next?.Next == null)
                return;

            for (var node = ll.First.Next; node?.Next != null && node.Previous != null; node = node.Next)
            {
                if (CanRemove(node.Previous.Value, node.Value, node.Next.Value))
                {
                    var n = node.Previous;
                    ll.Remove(node);
                    node = n;
                }
            }

            keyframes.Clear();
            keyframes.AddRange(ll);
        }

        private bool CanRemove(in KeyFrame a, in KeyFrame b, in KeyFrame c)
        {
            const float epsilon = 0.003f;

            // Determine how far between "A" and "C" "B" is
            var t = (float)((b.Time.TotalSeconds - a.Time.TotalSeconds) / (c.Time.TotalSeconds - a.Time.TotalSeconds));

            // Estimate where B *should* be using purely Lerp(a, c, t)
            var estimation = Estimate(a.Value, c.Value, t);

            // Calculate error
            var err = Error(b.Value, estimation);

            return err < epsilon;
        }

        public void Serialize(JsonWriter writer)
        {
            writer.WriteStartObject();
            {
                KeyframeReduction(_keyframes);

                writer.WritePropertyName("Name");
                writer.WriteValue(_name);

                writer.WritePropertyName("Type");
                writer.WriteValue(typeof(T).Name);

                writer.WritePropertyName("Keys");
                writer.WriteStartArray();
                {
                    foreach (var item in _keyframes)
                    {
                        writer.WriteStartObject();
                        {
                            writer.WritePropertyName("T");
                            writer.WriteValue((ulong)item.Time.TotalMilliseconds);
                            WriteKeyframeElements(writer, item.Value);
                        }
                        writer.WriteEndObject();
                    }
                }
                writer.WriteEndArray();
            }
            writer.WriteEndObject();
        }
    }
}
