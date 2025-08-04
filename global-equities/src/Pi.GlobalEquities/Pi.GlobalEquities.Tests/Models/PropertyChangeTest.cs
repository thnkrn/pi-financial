// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Pi.GlobalEquities.Models;
using Xunit;

namespace Pi.GlobalEquities.Tests.Models;

public class PropertyChangeTest
{
    public class CheckChange_Tests
    {
        [Fact]
        public void CheckChange_ForSingleValueButInputCollectionType_ThrowsArgumentException()
        {
            // Arrange
            var changes = new List<PropertyChange>();
            var oldValue = new List<int> { 1, 2, 3 };
            var newValue = new List<int> { 1, 2, 3, 4 };

            // Act and Assert
            Assert.Throws<ArgumentException>(() => PropertyChange.CheckChange("TestProperty", oldValue, newValue, changes));
        }

        [Fact]
        public void CheckChange_ForIEnumerableType_ReturnsTrue()
        {
            // Arrange
            var changes = new List<PropertyChange>();
            IEnumerable<int> oldValue = new List<int> { 1, 2, 3 };
            IEnumerable<int> newValue = new List<int> { 1, 2, 3, 4 };
            var propName = "MockName";

            // Act
            var result = PropertyChange.CheckChange(propName, oldValue, newValue, changes);

            // Assert
            Assert.True(result);
            Assert.Single(changes);
            Assert.Equal(propName, changes[0].Name);
            Assert.Equal(oldValue, changes[0].OldValue);
            Assert.Equal(newValue, changes[0].NewValue);
        }

        [Fact]
        public void CheckChange_ForSingleValue_ReturnsTrue()
        {
            // Arrange
            var changes = new List<PropertyChange>();
            var oldValue = "OldValue";
            var newValue = "NewValue";
            var propName = "MockName";

            // Act
            var result = PropertyChange.CheckChange(propName, oldValue, newValue, changes);

            // Assert
            Assert.True(result);
            Assert.Single(changes);
            Assert.Equal(propName, changes[0].Name);
            Assert.Equal(oldValue, changes[0].OldValue);
            Assert.Equal(newValue, changes[0].NewValue);
        }


        [Fact]
        public void CheckChange_ForIEnumerableTypeNoChange_ReturnsFalse()
        {
            // Arrange
            var changes = new List<PropertyChange>();
            IEnumerable<int> oldValue = new List<int> { 1, 2, 3 };
            IEnumerable<int> newValue = new List<int> { 1, 2, 3 };
            var propName = "MockName";

            // Act
            var result = PropertyChange.CheckChange(propName, oldValue, newValue, changes);

            // Assert
            Assert.False(result);
            Assert.Empty(changes);
        }

        [Fact]
        public void CheckChange_ForSingleValue_NoChange_ReturnsFalse()
        {
            // Arrange
            var changes = new List<PropertyChange>();
            var oldValue = "Value";
            var newValue = "Value";
            var propName = "MockName";

            // Act
            var result = PropertyChange.CheckChange(propName, oldValue, newValue, changes);

            // Assert
            Assert.False(result);
            Assert.Empty(changes);
        }

        [Fact]
        public void CheckChange_List_NullValues_ReturnsFalse()
        {
            // Arrange
            var changes = new List<PropertyChange>();
            IEnumerable<int> oldValue = null;
            IEnumerable<int> newValue = null;
            var propName = "MockName";

            // Act
            var result = PropertyChange.CheckChange(propName, oldValue, newValue, changes);

            // Assert
            Assert.False(result);
            Assert.Empty(changes);
        }

        [Fact]
        public void CheckChange_ForSingleNullValue_ReturnsFalse()
        {
            // Arrange
            var changes = new List<PropertyChange>();
            string oldValue = null;
            string newValue = null;
            var propName = "MockName";

            // Act
            var result = PropertyChange.CheckChange(propName, oldValue, newValue, changes);

            // Assert
            Assert.False(result);
            Assert.Empty(changes);
        }
    }
}
