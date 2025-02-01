namespace Forest
{
    public class TimedModifier : Timed
    {
        public GameContext Context;
        public ModifierData Modifier;

        public override double Duration => Modifier.Duration;
        public override bool IsLooping => false;

        public TimedModifier(GameContext context, ModifierData modifier)
        {
            Context = context;
            Modifier = modifier;
        }

        public override void Start()
        {
            foreach (Modifier m in Modifier.InstantModifier)
            {
                Context.AddModifier(m);
            }

            foreach (Modifier m in Modifier.OnTickModifier)
            {
                Context.AddTick(m);
            }

            foreach (Modifier m in Modifier.OnActionModifier)
            {
                Context.AddAction(m);
            }
        }

        public override void Expire()
        {
            foreach (Modifier m in Modifier.InstantModifier)
            {
                Context.RemoveModifier(m);
            }

            foreach (Modifier m in Modifier.OnTickModifier)
            {
                Context.RemoveTick(m);
            }

            foreach (Modifier m in Modifier.OnActionModifier)
            {
                Context.RemoveAction(m);
            }
        }
    }
}
