namespace TaskCore.ValueObjects
{
    public record TaskDeadline : IComparable<TaskDeadline>
    {
        public DateTime Value { get; }

        private TaskDeadline(DateTime value) => Value = value;

        public static TaskDeadline Of(DateTime value)
        {
            if (value <= DateTime.UtcNow)
                throw new ArgumentException("Task deadline must be in the future.");

            return new TaskDeadline(value);
        }

        public static TaskDeadline FromDatabase(DateTime value)
        {
            // هنا لا نتحقق من الزمن، لأننا نقرأ حقيقة تاريخية قد تكون قديمة
            return new TaskDeadline(value);
        }
        public bool IsPassed() => DateTime.UtcNow > Value;

        public override string ToString() => Value.ToString("O");

        // ✅ 2. ضروري جداً: CompareTo للترتيب
        public int CompareTo(TaskDeadline? other)
        {
            if (other is null) return 1;
            return Value.CompareTo(other.Value);
        }

        // ✅ 3. الأهم: Operators لتمكين البحث في EF Core
        // هذا ما سيجعل جملة LINQ تترجم إلى SQL بشكل صحيح
        public static bool operator <(TaskDeadline left, TaskDeadline right)
            => left is not null && right is not null && left.Value < right.Value;

        public static bool operator >(TaskDeadline left, TaskDeadline right)
            => left is not null && right is not null && left.Value > right.Value;

        public static bool operator <=(TaskDeadline left, TaskDeadline right)
            => left is not null && right is not null && left.Value <= right.Value;

        public static bool operator >=(TaskDeadline left, TaskDeadline right)
            => left is not null && right is not null && left.Value >= right.Value;
    }
}
