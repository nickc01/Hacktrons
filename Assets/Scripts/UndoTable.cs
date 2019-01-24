using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class UndoTable
{
    private List<Action> UndoList = new List<Action>();

    public void CommitAction(Action action,Action reverse)
    {
        UndoList.Add(reverse);
        action();
    }
    public void Undo()
    {
        UndoList[UndoList.Count - 1]();
        UndoList.RemoveAt(UndoList.Count - 1);
    }
}
