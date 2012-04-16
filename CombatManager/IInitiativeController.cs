using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CombatManager
{
    public interface IInitiativeController
    {
        void MoveNextPlayer();
        void MovePreviousPlayer();

        void MoveUpCharacter(Character character);
        void MoveDownCharacter(Character character);
        void MoveCharacterAfter(Character charMove, Character charAfter);

        void RollInitiative();
        void SortInitiative();
    }
}
