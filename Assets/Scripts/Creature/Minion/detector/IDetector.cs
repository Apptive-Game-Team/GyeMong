using System.Collections.Generic;

public interface IDetector<T>
{
    public List<T> DetectTargets();
    public T DetectTarget();
}
