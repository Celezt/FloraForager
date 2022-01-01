using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Celezt.Timeline
{
    [TrackColor(1, 1, 0)]
    [TrackBindingType(typeof(ParticleSystem))]
    [TrackClipType(typeof(ParticleSystemAsset))]
    public class ParticleSystemTrack : TrackAsset
    {
        public ParticleSystemMixerBehaviour Template = new ParticleSystemMixerBehaviour();

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            ScriptPlayable<ParticleSystemMixerBehaviour> playable = ScriptPlayable<ParticleSystemMixerBehaviour>.Create(graph, Template, inputCount);
            return playable;
        }
    }
}
