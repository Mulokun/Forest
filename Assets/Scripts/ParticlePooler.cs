namespace Forest
{
    public class ParticlePooler : Pooler<ParticleHandler>
    {
        public ParticlePooler(ParticleHandler prefab, int base_count = 1) :
            base(prefab, base_count)
        {
        }

        protected override ParticleHandler Create()
        {
            ParticleHandler o = base.Create();
            o.ParentPooler = this;
            return o;
        }

        public override ParticleHandler Borrow()
        {
            ParticleHandler o = base.Borrow();
            o.gameObject.SetActive(true);
            return o;
        }
    }

}
