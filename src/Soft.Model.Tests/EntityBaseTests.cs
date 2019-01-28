using Soft.Model.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Soft.Model.Tests
{
    public class EntityBaseTests
    {
        /// <summary>
        /// Subject under test must be derived from abstract class EntityBase
        /// </summary>
        private class TestEntity : EntityBase
        {
            public TestEntity()
            {
            }
            public TestEntity(int id)
            {
                Id = id;
            }
        }
        /// <summary>
        /// A second different type derived from abstract class EntityBase is required for the Equals Tests
        /// </summary>
        private class DifferentTestEntity : EntityBase
        {
            public DifferentTestEntity()
            {
            }
            public DifferentTestEntity(int id)
            {
                Id = id;
            }
        }

        [Fact]
        public void Equals_When_OtherNotEntityBase_ShouldBe_False()
        {
            TestEntity testEntity = new TestEntity();
            int noEntityBaseTypeHereInt = 32;
            Assert.False(testEntity.Equals(noEntityBaseTypeHereInt));
        }
        [Fact]
        public void Equals_When_ReferenceEqual_ShouldBe_True()
        {
            TestEntity testEntity = new TestEntity();
            TestEntity referenceEqualTestEntity = testEntity;
            Assert.True(testEntity.Equals(referenceEqualTestEntity));
        }
        [Fact]
        public void Equals_When_OtherEntityOfOtherType_ShouldBe_False()
        {
            TestEntity testEntity = new TestEntity();
            DifferentTestEntity differentTestEntityType = new DifferentTestEntity();
            Assert.False(testEntity.Equals(differentTestEntityType));
        }
        [Fact]
        public void Equals_When_EntityIdIs0_ShouldBe_False()
        {
            TestEntity testEntityId0 = new TestEntity(id: 0);
            TestEntity testEntityValidId = new TestEntity(id: 10);
            Assert.False(testEntityId0.Equals(testEntityValidId));
        }

        [Fact]
        public void Equals_When_OtherEntityIdIs0_ShouldBe_False()
        {
            TestEntity testEntityValidId = new TestEntity(id: 10);
            TestEntity testEntityId0 = new TestEntity(id: 0);
            Assert.False(testEntityValidId.Equals(testEntityId0));
        }

        [Fact]
        public void Equals_When_OtherEntityIsEqual_ShouldBe_True()
        {
            TestEntity testEntityValidId = new TestEntity(id: 10);
            TestEntity anotherTestEntitySameId = new TestEntity(id: testEntityValidId.Id);
            Assert.True(testEntityValidId.Equals(anotherTestEntitySameId));
        }

        [Fact]
        public void EqualsOperator_When_BothNull_ShouldBe_True()
        {
            TestEntity testEntityOfNull = null;
            TestEntity anotherTestEntityOfNull = null;
            Assert.True(testEntityOfNull == anotherTestEntityOfNull);
        }

        [Fact]
        public void EqualsOperator_When_EntityNull_ShouldBe_False()
        {
            TestEntity testEntityOfNull = null;
            TestEntity testEntityValidId = new TestEntity(id: 10);
            Assert.False(testEntityOfNull == testEntityValidId);
        }

        [Fact]
        public void EqualsOperator_When_OtherEntityNull_ShouldBe_False()
        {
            TestEntity testEntityValidId = new TestEntity(id: 10);
            TestEntity testEntityOfNull = null;
            Assert.False(testEntityValidId == testEntityOfNull);
        }

        [Fact]
        public void EqualsOperator_When_BothEqual_ShouldBe_True()
        {
            TestEntity testEntityValidId = new TestEntity(id: 10);
            TestEntity anotherTestEntitySameId = new TestEntity(id: testEntityValidId.Id);
            Assert.True(testEntityValidId == anotherTestEntitySameId);
        }

        [Fact]
        public void NotEqualsOperator_When_BothEqual_ShouldBe_False()
        {
            TestEntity testEntityValidId = new TestEntity(id: 10);
            TestEntity anotherTestEntitySameId = new TestEntity(id: testEntityValidId.Id);
            Assert.False(testEntityValidId != anotherTestEntitySameId);
        }

        [Fact]
        public void NotEqualsOperator_When_BothNotEqual_ShouldBe_True()
        {
            TestEntity testEntityValidId = new TestEntity(id: 10);
            TestEntity anotherTestEntitySameId = new TestEntity(id: 11);
            Assert.True(testEntityValidId != anotherTestEntitySameId);
        }

        [Fact]
        public void GetHashCode_When_TestEntityWithId_Should_Return()
        {
            TestEntity testEntityValidId = new TestEntity(id: 10);

            var help = (testEntityValidId.GetType().ToString() + testEntityValidId.Id);
            Assert.Equal("Soft.Model.Tests.EntityBaseTests+TestEntity10", help);
            var hcode = help.GetHashCode();
            Assert.Equal(hcode, testEntityValidId.GetHashCode());
        }
    }
}
