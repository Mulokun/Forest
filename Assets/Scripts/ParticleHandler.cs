using UnityEngine;

namespace Forest
{
    public class ParticleHandler : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particle;
        public ParticlePooler ParentPooler { get; set; } = null;

        protected void OnParticleSystemStopped()
        {
            ParentPooler.Return(this);
        }

        protected void OnEnable()
        {
            particle.Play();
        }

        protected void OnDisable()
        {
            particle.Stop();
        }
    }
}
