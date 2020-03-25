using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DIV2Tools.DIVFormats
{
    /// <summary>
    /// Base logic shared with all DIV format implementations.
    /// </summary>
    public abstract class DIVFormatBase
    {
        #region Internal vars
        /// <summary>
        /// <see cref="DIVFormatBaseHeader"/> header instance.
        /// </summary>
        protected DIVFormatBaseHeader header;
        #endregion

        #region Public vars
        /// <summary>
        /// Marks if <see cref="DIVFormatBase.CloseBinaryReader(BinaryReader)"/> close the <see cref="BinaryReader"/> instance passed as argument when is called.
        /// </summary>
        public bool closeBinaryReader; 
        #endregion

        #region Methods & Functions
        /// <summary>
        /// Closes the <see cref="BinaryReader"/> instance passed as argument if <see cref="DIVFormatBase.closeBinaryReader"/> mark is <see cref="true"/>.
        /// </summary>
        /// <param name="stream"><see cref="BinaryReader"/> instance.</param>
        public void CloseBinaryReader(BinaryReader stream)
        {
            if (this.closeBinaryReader)
            {
                this.closeBinaryReader = false;
                stream.Close();
            }
        }

        /// <summary>
        /// Remove header data.
        /// </summary>
        public void RemoveHeader()
        {
            this.header = null;
        }
        #endregion
    }
}
