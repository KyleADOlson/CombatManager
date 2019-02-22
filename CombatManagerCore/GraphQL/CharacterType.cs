using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.GraphQL
{
    class CharacterType : ObjectGraphType<Character>
    {
        public CharacterType()
        {
            Field(x => x.Name).Description("The Character's name");
            Field(x => x.ID).Description("The Character's ID");
            Field(x => x.HP).Description("The Character's HP");
        }
    }
}
