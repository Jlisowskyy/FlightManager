namespace proj.Interfaces;

public interface IVisitable
{
    public bool ProcessVisitor(IVisitor visitor);
}