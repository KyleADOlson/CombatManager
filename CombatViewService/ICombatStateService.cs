using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CombatManager;

namespace CombatViewService
{
    [ServiceContract(CallbackContract = typeof(ICombatStateCallback))]
    public interface ICombatStateService
    {
        [OperationContract]
        List<SimpleCombatListItem> GetCombatList();
        [OperationContract]
        List<Character> GetCharacters();
        [OperationContract]
        Character GetCurrentCharacter();
        [OperationContract]
        Guid GetCurrentCharacterID();
        [OperationContract]
        int? GetRound();

    }

    public interface ICombatStateCallback
    {
        [OperationContract]
        void CurrentPlayerChanged(Guid id);

        [OperationContract]
        void CombatListChanged();

        [OperationContract]
        void CharactersChanged();
    }


}
