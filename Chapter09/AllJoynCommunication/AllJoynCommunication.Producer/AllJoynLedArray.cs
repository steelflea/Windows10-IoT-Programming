using AllJoynCommunication.Producer.Enums;
using AllJoynCommunication.Producer.SenseHatLedArray;
using com.iot.SenseHatLedArray;
using System;
using System.Threading.Tasks;
using Windows.Devices.AllJoyn;
using Windows.Foundation;
using Windows.UI;

namespace AllJoynCommunication.Producer
{
    public sealed class AllJoynLedArray : ISenseHatLedArrayService
    {
        private ShapeKind currentShape = ShapeKind.None;

        private LedArray ledArray;

        public AllJoynLedArray(LedArray ledArray)
        {
            this.ledArray = ledArray;
        }

        public IAsyncOperation<SenseHatLedArrayDrawShapeResult> DrawShapeAsync(
            AllJoynMessageInfo info, int interfaceMemberShapeKind)
        {
            Task<SenseHatLedArrayDrawShapeResult> task = 
                new Task<SenseHatLedArrayDrawShapeResult>(() =>
            {
                if (ledArray != null)
                {
                    currentShape = GetShapeKind(interfaceMemberShapeKind);

                    ledArray.DrawShape(currentShape);

                    return SenseHatLedArrayDrawShapeResult.CreateSuccessResult();
                }
                else
                {
                    return SenseHatLedArrayDrawShapeResult.CreateFailureResult(
                        (int)ErrorCodes.LedInitializationError);
                }
            });

            task.Start();

            return task.AsAsyncOperation();
        }

        public IAsyncOperation<SenseHatLedArrayGetShapeResult> GetShapeAsync(AllJoynMessageInfo info)
        {
            Task<SenseHatLedArrayGetShapeResult> task = new Task<SenseHatLedArrayGetShapeResult>(() =>
            {
                return SenseHatLedArrayGetShapeResult.CreateSuccessResult((byte)currentShape);
            });

            task.Start();

            return task.AsAsyncOperation();
        }

        public IAsyncOperation<SenseHatLedArrayTurnOffResult> TurnOffAsync(AllJoynMessageInfo info)
        {
            Task<SenseHatLedArrayTurnOffResult> task = new Task<SenseHatLedArrayTurnOffResult>(() =>
            {
                if (ledArray != null)
                {
                    ledArray.Reset(Colors.Black);

                    return SenseHatLedArrayTurnOffResult.CreateSuccessResult();
                }
                else
                {
                    return SenseHatLedArrayTurnOffResult.CreateFailureResult((int)ErrorCodes.LedInitializationError);
                }
            });

            task.Start();

            return task.AsAsyncOperation();
        }

        private ShapeKind GetShapeKind(int intShapeKind)
        {
            var shapeKind = ShapeKind.None;

            if (Enum.IsDefined(typeof(ShapeKind), intShapeKind))
            {
                shapeKind = (ShapeKind)intShapeKind;
            }

            return shapeKind;
        }
    }
}
