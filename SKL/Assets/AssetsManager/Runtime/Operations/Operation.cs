using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets
{
    public enum OperationStatus
    {
        Idle,
        Processing,
        Success,
        Failed
    }

    public class Operation : IEnumerator
    {
        internal static readonly List<Operation> Processing = new List<Operation>();
        public Action<Operation> completed;
        public OperationStatus status { get; protected set; } = OperationStatus.Idle;
        public float progress { get; protected set; }
        public bool isDone => status == OperationStatus.Failed || status == OperationStatus.Success;
        public string error { get; protected set; }
        public string description { protected set; get; }

        public bool MoveNext()
        {
            return !isDone;
        }

        public void Reset()
        {
        }

        public object Current => null;

        internal static void Process(Operation operation)
        {
            operation.status = OperationStatus.Processing;
            Processing.Add(operation);
        }

        protected virtual void Update()
        {
        }

        public virtual void Start()
        {
            status = OperationStatus.Processing;
            Process(this);
        }

        public void Cancel()
        {
            Error("User Cancel.");
        }

        protected void Error(string errorCode = null)
        {
            error = errorCode;
            progress = 1;
            status = OperationStatus.Failed;
        }

        protected void Finish()
        {
            error = null;
            progress = 1;
            status = OperationStatus.Success;
        }

        protected void Complete()
        {
            if (completed == null) return;

            var saved = completed;
            completed.Invoke(this);
            completed -= saved;
        }

        public static void UpdateAll()
        {
            for (var index = 0; index < Processing.Count; index++)
            {
                var item = Processing[index];
                if (Updater.Busy())
                    return;

                item.Update();
                if (!item.isDone) 
                    continue;

                Processing.RemoveAt(index);
                index--;
                if (item.status == OperationStatus.Failed)
                    Logger.W("Unable to complete {0} with error: {1}", item.GetType().Name, item.error);

                item.Complete();
            }

            InstantiateObject.UpdateObjects();
        }
    }
}