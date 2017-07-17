using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodTimeStudio.ServerPing.Motd
{
    class TextComponentBase : IEnumerable<TextComponentBase>
    {
        /// <summary>
        /// The later siblings of this component.  If this component turns the text bold, that will apply to all the siblings
        /// until a later sibling turns the text something else.
        /// </summary>
        public List<TextComponentBase> siblings = new List<TextComponentBase>();

        public IEnumerator<TextComponentBase> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
