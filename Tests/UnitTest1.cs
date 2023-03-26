using Moq;
using NUnit.Framework;
using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.DataAccessLayer;
using System;

namespace Tests
{
    public class Tests
    {
        Column column;

        [SetUp]
        public void Setup()
        {
            var columnController = new Mock<ColumnController>();
            var columnDTO = new Mock<IntroSE.Kanban.Backend.DataAccessLayer.DTOs.IColumn>();
            columnDTO.Setup(column => column.Id).Returns(1);
            columnDTO.Setup(column => column.Title).Returns("backlog");
            columnDTO.Setup(column => column.Limit).Returns(-1);
            columnDTO.Setup(column => column.Ordinal).Returns(0);
            column = new Column(columnController.Object, columnDTO.Object);
        }

        /*
         * We chose to cover three Column class methods:
         *   - AddTask
         *   - GetTask
         *   - RemoveTask
         */

        [Test]
        public void AddTask()
        {
            var task = new Mock<ITask>();
            task.Setup(task => task.Id).Returns(1);
            column.AddTask(task.Object);
            Assert.AreSame(task.Object, column.GetTask(1));
        }

        [Test]
        public void AddTaskNull()
        {
            Assert.Throws<ArgumentException>(() => { column.AddTask(null); }, "Should throw when adding null Task.");
        }

        [Test]
        public void AddTaskOverLimit()
        {
            column.Limit = 1;
            var task1 = new Mock<ITask>();
            task1.Setup(task => task.Id).Returns(1);
            column.AddTask(task1.Object);
            var task2 = new Mock<ITask>();
            task2.Setup(task => task.Id).Returns(2);
            Assert.Throws<InvalidOperationException>(() => { column.AddTask(task2.Object); }, "Should throw when adding Task above limit.");
        }

        [Test]
        public void AddTaskAlreadyInside()
        {
            var task = new Mock<ITask>();
            task.Setup(task => task.Id).Returns(1);
            column.AddTask(task.Object);
            Assert.Throws<ArgumentException>(() => { column.AddTask(task.Object); }, "Should throw when adding Task with same Id.");
        }

        [Test]
        public void GetTask()
        {
            var taskMock = new Mock<ITask>();
            taskMock.Setup(task => task.Id).Returns(1);
            column.AddTask(taskMock.Object);
            var task = column.GetTask(1);
            Assert.AreEqual(1, task.Id);
        }

        [Test]
        public void GetTaskDoesNotExist()
        {
            Assert.Throws<ArgumentException>(() => { column.GetTask(1); }, "Should throw when getting Task that does not exist.");
        }

        [Test]
        public void RemoveTask()
        {
            var taskMock = new Mock<ITask>();
            taskMock.Setup(task => task.Id).Returns(1);
            column.AddTask(taskMock.Object);
            var task = column.RemoveTask(1);
            Assert.AreEqual(1, task.Id);
        }

        [Test]
        public void RemoveTaskDoesNotExist()
        {
            Assert.Throws<ArgumentException>(() => { column.RemoveTask(1); }, "Should throw when removing Task that does not exist.");
        }
    }
}
