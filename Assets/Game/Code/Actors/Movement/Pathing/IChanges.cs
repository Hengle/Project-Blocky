namespace ProjectBlocky.Actors.Pathing
{
    public interface IChanges<T>
    {
#if UNITY_EDITOR
        void InitializeValues(T target);
#endif
        void ApplyChanges(T target);
    }
}