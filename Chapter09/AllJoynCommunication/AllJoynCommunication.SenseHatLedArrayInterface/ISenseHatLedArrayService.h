//-----------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by a tool.
//
//   Changes to this file may cause incorrect behavior and will be lost if
//   the code is regenerated.
//
//   For more information, see: http://go.microsoft.com/fwlink/?LinkID=623246
// </auto-generated>
//-----------------------------------------------------------------------------
#pragma once

namespace com { namespace iot { namespace SenseHatLedArray {

public interface class ISenseHatLedArrayService
{
public:
    // Implement this function to handle calls to the DrawShape method.
    Windows::Foundation::IAsyncOperation<SenseHatLedArrayDrawShapeResult^>^ DrawShapeAsync(_In_ Windows::Devices::AllJoyn::AllJoynMessageInfo^ info , _In_ int32 interfaceMemberShapeKind);

    // Implement this function to handle calls to the TurnOff method.
    Windows::Foundation::IAsyncOperation<SenseHatLedArrayTurnOffResult^>^ TurnOffAsync(_In_ Windows::Devices::AllJoyn::AllJoynMessageInfo^ info );

    // Implement this function to handle requests for the value of the Shape property.
    //
    // Currently, info will always be null, because no information is available about the requestor.
    Windows::Foundation::IAsyncOperation<SenseHatLedArrayGetShapeResult^>^ GetShapeAsync(Windows::Devices::AllJoyn::AllJoynMessageInfo^ info);

};

} } } 
