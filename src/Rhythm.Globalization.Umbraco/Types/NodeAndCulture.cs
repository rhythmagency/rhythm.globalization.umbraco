namespace Rhythm.Globalization.Umbraco.Types
{

    /// <summary>
    /// Stores a node ID and a culture code.
    /// </summary>
    /// <remarks>
    /// Equality is checked by comparing the node ID and culture code of two
    /// instances of this class.
    /// </remarks>
    internal class NodeAndCulture
    {

        #region Properties

        /// <summary>
        /// The node ID.
        /// </summary>
        public int NodeId { get; set; }

        /// <summary>
        /// The culture code.
        /// </summary>
        public string Culture { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Primary constructor.
        /// </summary>
        /// <param name="nodeId">
        /// The node ID.
        /// </param>
        /// <param name="culture">
        /// The culture code.
        /// </param>
        public NodeAndCulture(int nodeId, string culture)
        {
            this.NodeId = nodeId;
            this.Culture = culture;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Compares this instance against another for equality.
        /// </summary>
        /// <param name="other">
        /// The other instance.
        /// </param>
        /// <returns>
        /// True, if the instances are equal; otherwise, false.
        /// </returns>
        public bool Equals(NodeAndCulture other)
        {
            if (other == null)
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return this.NodeId == other.NodeId && this.Culture == other.Culture;
        }

        /// <summary>
        /// Compares this instance against another for equality.
        /// </summary>
        /// <param name="other">
        /// The other instance.
        /// </param>
        /// <returns>
        /// True, if the instances are equal; otherwise, false.
        /// </returns>
        public override bool Equals(object other)
        {
            return this.Equals(other as NodeAndCulture);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// The hash code.
        /// </returns>
        public override int GetHashCode()
        {
            return NodeId.GetHashCode() ^ (Culture ?? string.Empty).GetHashCode();
        }

        #endregion

    }

}