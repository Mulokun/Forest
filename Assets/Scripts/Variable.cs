namespace Forest
{
    public delegate void ValueUpdated(double newValue, double previousValue);
    public delegate double ValueMinMax(double value);

    public class Variable
    {
        private double number;
        private double numberModified;

        public double ModifiedValue
        {
            get
            {
                return numberModified;
            }

            set
            {
                double previous = numberModified;
                numberModified = MinMax != null ? MinMax(value) : value;
                OnUpdate?.Invoke(numberModified, previous);
            }
        }

        public double BaseValue
        {
            get
            {
                return number;
            }

            set
            {
                double previous = number;
                number = MinMax != null ? MinMax(value) : value;
                OnUpdateBase?.Invoke(number, previous);
            }
        }

        public ValueMinMax MinMax;
        public event ValueUpdated OnUpdateBase;
        public event ValueUpdated OnUpdate;

        public Variable(double init_value)
        {
            number = init_value;
            ModifiedValue = number;
        }

        public override string ToString()
        {
            return ToFormatedString();
        }

        public string ToFormatedString(int zeros = 0)
        {
            if (number != ModifiedValue)
            {
                return ModifiedValue.ToFormatedString(zeros) + " (" + number.ToFormatedString(zeros) + ")";
            }
            return number.ToFormatedString(zeros);
        }
    }
}
