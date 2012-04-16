using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatManager
{
    public class UndoHelper
    {
        LinkedList<UndoAction> undoStack = new LinkedList<UndoAction>();
        LinkedList<UndoAction> redoStack = new LinkedList<UndoAction>();
        LinkedList<UndoAction> tempRedoStack = new LinkedList<UndoAction>();

        public bool IsOwned
        {
            get
            {
                return ownershipLevel > 0;
            }
        }

        int undoGroupLevel = 0;
        int ownershipLevel = 0;

        public void AddToUndo(UndoAction act)
        {
            if (!IsOwned)
            {
                if (redoStack.Count > 0)
                {
                    tempRedoStack = redoStack;
                    redoStack = new LinkedList<CombatManager.UndoAction>();
                }

                if (undoStack.Count == 0 ||
                    !undoStack.Last.Value.Equals(act))
                {
                    undoStack.AddLast(act);
                }

                while (undoStack.Count > 5000)
                {
                    undoStack.RemoveFirst();
                }
            }
        }

        void ClearGroupIfNeccessary()
        {
            bool resetRedo = false;

            if (undoStack.Count >= 2)
            {
                if ((undoStack.Last.Value.UndoType == CombatManager.UndoAction.UndoTypes.UndoGroupEnd &&
                    undoStack.Last.Previous.Value.UndoType == CombatManager.UndoAction.UndoTypes.UndoGroupStart) ||
                    (undoStack.Last.Value.UndoType == CombatManager.UndoAction.UndoTypes.UndoGroupStart &&
                    undoStack.Last.Previous.Value.UndoType == CombatManager.UndoAction.UndoTypes.UndoGroupEnd))
                {
                    undoStack.RemoveLast();
                    undoStack.RemoveLast();

                    if (redoStack.Count == 0 && tempRedoStack.Count > 0)
                    {
                        resetRedo = true;
                    }
                }
            }

            if (resetRedo)
            {
                redoStack = tempRedoStack;
                tempRedoStack = new LinkedList<UndoAction>();
            }
            else
            {
                tempRedoStack.Clear();
            }
        }

        public List<UndoAction> UndoAction()
        {
            List<UndoAction> ret = new List<UndoAction>();

            if (undoStack.Count > 0)
            {
                UndoAction act = undoStack.Last.Value;
                undoStack.RemoveLast();
                redoStack.AddLast(act);
                ret.Add(act);

                if (act.UndoType == CombatManager.UndoAction.UndoTypes.UndoGroupEnd)
                {
                    while (true)
                    {
                        if (undoStack.Count == 0)
                        {
                            break;
                        }
                        act = undoStack.Last.Value;
                        undoStack.RemoveLast();
                        redoStack.AddLast(act);
                        ret.Add(act);

                        if (act.UndoType == CombatManager.UndoAction.UndoTypes.UndoGroupStart)
                        {
                            break;
                        }
                    }
                }
            }

            return ret;
        }

        public List<UndoAction> RedoAction()
        {
            List<UndoAction> ret = new List<UndoAction>();

            if (redoStack.Count > 0)
            {
                UndoAction act = redoStack.Last.Value;
                redoStack.RemoveLast();
                undoStack.AddLast(act);
                ret.Add(act);

                if (act.UndoType == CombatManager.UndoAction.UndoTypes.UndoGroupStart)
                {
                    while (true)
                    {
                        if (redoStack.Count == 0)
                        {
                            break;
                        }
                        act = redoStack.Last.Value;
                        redoStack.RemoveLast();
                        undoStack.AddLast(act);
                        ret.Add(act);

                        if (act.UndoType == CombatManager.UndoAction.UndoTypes.UndoGroupEnd)
                        {
                            break;
                        }
                    }
                }
            }

            return ret;
        }

        public Helper TakeOwner()
        {
            return new Helper(this);
        }

        public UndoGroupHelper CreateUndoGroup()
        {
            return new UndoGroupHelper(this);
        }

        public class UndoGroupHelper : IDisposable
        {
            UndoHelper owner;

            public UndoGroupHelper(UndoHelper owner)
            {
                this.owner = owner;

                if (owner.undoGroupLevel == 0)
                {
                    owner.AddToUndo(new UndoAction(true));
                }

                owner.undoGroupLevel++;
            }

            public void Dispose()
            {
                if (owner.undoGroupLevel == 1)
                {
                    owner.AddToUndo(new UndoAction(false));

                    owner.ClearGroupIfNeccessary();
                }

                owner.undoGroupLevel--;
            }
        }

        public class Helper : IDisposable
        {
            UndoHelper owner;

            internal Helper(UndoHelper owner)
            {
                this.owner = owner;

                owner.ownershipLevel++;
            }

            public void Dispose()
            {
                owner.ownershipLevel--;
            }
        }
    }
}
