using System;
using System.Linq;
using Intersect.Enums;
using Intersect.Models;

using Newtonsoft.Json;

namespace Intersect.GameObjects
{
    public partial class GuildVariableBase : DatabaseObject<GuildVariableBase>, IFolderable
    {
        [JsonConstructor]
        public GuildVariableBase(Guid id) : base(id)
        {
            Name = "New Guild Variable";
        }

        public GuildVariableBase()
        {
            Name = "New Guild Variable";
        }

        //Identifier used for event chat variables to display the value of this variable/switch.
        //See https://www.ascensiongamedev.com/topic/749-event-text-variables/ for usage info.
        public string TextId { get; set; }

        // TODO(0.8): Rename this to DataType
        public VariableDataType Type { get; set; } = VariableDataType.Boolean;

        /// <inheritdoc />
        public string Folder { get; set; } = string.Empty;

        /// <summary>
        /// Retrieve an array of variable names of the supplied data type.
        /// </summary>
        /// <param name="dataType">The data type to retrieve names of.</param>
        /// <returns>Returns an array of names.</returns>
        public static string[] GetNamesByType(VariableDataType dataType) =>
            Lookup
                .Where(pair => pair.Value is GuildVariableBase descriptor && descriptor.Type == dataType)
                .OrderBy(pair => pair.Value.TimeCreated)
                .Select(pair => pair.Value.Name)
                .ToArray();

        /// <summary>
        /// Retrieve the list index of an Id within a specific data type list.
        /// </summary>
        /// <param name="id">The Id to look up.</param>
        /// <param name="dataType">The data type to search up.</param>
        /// <returns>Returns the list Index of the provided Id.</returns>
        public static int ListIndex(Guid id, VariableDataType dataType) =>
            Lookup
                .Where(pair => pair.Value is GuildVariableBase descriptor && descriptor.Type == dataType)
                .OrderBy(pair => pair.Value.TimeCreated)
                .ToList()
                .FindIndex(pair => pair.Value.Id == id);

        /// <summary>
        /// Retrieve the Id associated with a list index of a specific data type.
        /// </summary>
        /// <param name="listIndex">The list index to retrieve.</param>
        /// <param name="dataType">The data type to search up.</param>
        /// <returns>Returns the Id of the provided index.</returns>
        public static Guid IdFromList(int listIndex, VariableDataType dataType)
        {
            if (listIndex < 0 || listIndex > GetNamesByType(dataType).Length)
            {
                return Guid.Empty;
            }

            return Lookup
                .Where(pair => pair.Value is GuildVariableBase descriptor && descriptor.Type == dataType)
                .OrderBy(pair => pair.Value.TimeCreated)
                .Skip(listIndex)
                .First()
                .Value.Id;
        }
    }
}
