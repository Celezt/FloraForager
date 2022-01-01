using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Celezt.Timeline
{
    [System.Serializable]
    public class ParticleSystemMixerBehaviour : PlayableBehaviour
    {
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            ParticleSystem trackBinding = playerData as ParticleSystem;
            bool isPLaying = false;

            if (!trackBinding)
                return;

            var rootPlayable = playable.GetGraph().GetRootPlayable(0);
            var time = (float)rootPlayable.GetTime();

            if (!Application.isPlaying && !trackBinding.isPlaying)
                PrepareParticleSystem(playable, trackBinding);

            int inputCount = playable.GetInputCount();
            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);

                if (inputWeight > 0.0f)
                {
                    ScriptPlayable<ParticleSystemBehaviour> inputPlayable = (ScriptPlayable<ParticleSystemBehaviour>)playable.GetInput(i);

                    ParticleSystemBehaviour input = inputPlayable.GetBehaviour();
                    isPLaying = true;
                }
            }
            if (Application.isPlaying)
            {
                if (isPLaying && !trackBinding.isPlaying)
                    trackBinding.Play();
                else if (!isPLaying && !trackBinding.isStopped)
                    trackBinding.Stop();
            }
            else
            {
                float minDelta = 1.0f / 240;
                float smallDelta = Mathf.Max(0.1f, Time.fixedDeltaTime * 2);
                float largeDelta = 0.2f;

                if (isPLaying && !trackBinding.isPlaying)
                    trackBinding.Play();
                else if (!isPLaying && !trackBinding.isStopped)
                    trackBinding.Stop();

                if (time < trackBinding.time ||
                    time > trackBinding.time + largeDelta)
                {
                    ResetSimulation(trackBinding, time);
                }
                else if (time > trackBinding.time + smallDelta)
                {
                    trackBinding.Simulate(time - trackBinding.time, true, false, true);
                }
                else if (time > trackBinding.time + minDelta)
                {
                    trackBinding.Simulate(time - trackBinding.time, true, false, false);
                }
                else
                    return;
            }
        }

        private void ResetSimulation(ParticleSystem particleSystem, float time)
        {
            const float maxSimTime = 2.0f / 3;

            if (time < maxSimTime)
                particleSystem.Simulate(time);
            else
            {
                particleSystem.Simulate(time - maxSimTime, true, true, false);
                particleSystem.Simulate(maxSimTime, true, false, true);
            }
        }

        private void PrepareParticleSystem(Playable playable, ParticleSystem particleSystem)
        {
            var rootPlayable = playable.GetGraph().GetRootPlayable(0);
            var duration = (float)rootPlayable.GetDuration();

            var main = particleSystem.main;
            if (main.duration < duration) main.duration = duration;
        }
    }
}
