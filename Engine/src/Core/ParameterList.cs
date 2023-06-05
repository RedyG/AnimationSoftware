using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    public class ParameterList : IReadOnlyList<NamedParameter>
    {
        [JsonProperty]
        private IReadOnlyList<NamedParameter> _parameters;

        public NamedParameter this[int index] => _parameters[index];

        public int Count => _parameters.Count;

        IEnumerator IEnumerable.GetEnumerator() => _parameters.GetEnumerator();

        public IEnumerator<NamedParameter> GetEnumerator() => _parameters.GetEnumerator();

        // Not using a hashmap because we need this to be ordered and 99% of the time we will only
        // iterate over the list to draw the UI so a list makes way more sense. Also these O(n)
        // methods will rarely be used. ( if ever )
        public Parameter? Get(string name)
        {
            foreach (NamedParameter namedParameter in _parameters)
            {
                if (namedParameter.Name == name)
                    return namedParameter.Parameter;
            }
            return null;
        }

        public ParameterList(params NamedParameter[] parameters)
        {
            _parameters = new List<NamedParameter>(parameters);
        }

        [JsonConstructor]
        public ParameterList(List<NamedParameter> parameters)
        {
            _parameters = parameters;
        }

        public ParameterList(IReadOnlyList<NamedParameter> parameters)
        {
            _parameters = parameters;
        }
    }
}
