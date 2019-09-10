# CEvent
This file contains the implementation of the CEvent class.

## Includes

````cpp
#include <cassert>
#include <time.h>

#include "platform/event.hpp"

using namespace std;
````

## Constants

The `NotSignaledValue` constant is used as the value for an event not signaled.

````cpp
const CEvent::EVENT_SUPPORT_TYPE CEvent::NotSignaledValue = CEvent::EVENT_SUPPORT_TYPE(false);
````

The `SignaledValue` constant is used as the value for a signaled event.

````cpp
const CEvent::EVENT_SUPPORT_TYPE CEvent::SignaledValue = CEvent::EVENT_SUPPORT_TYPE(true);
````

## Constructors and destructor

The class has only one constructor. The `mode` parameter indicates if the event should return to non signaled state after its signaled state is read.

````cpp
CEvent::CEvent(Mode mode)
    : m_Mode(mode)
{
    assert(mode == Manual || mode == AutoReset);

    m_EventHandle.store(NotSignaledValue);
}

CEvent::~CEvent()
{
}
````

## Client Interface

The class includes several method to get and set the event's state.

### Signal: signals an event

This command sets the event to the signaled state.

This is done by setting an atomic bool. In manual mode, the atomic bool is not reset, not until Reset() is called. In auto-reset mode, the bool is reset at the same time it is tested.

````cpp
void CEvent::Signal()
{
    m_EventHandle.store(SignaledValue);
}
````

### Reset: resets a signaled event.

The event is reset to the non signaled state. If the event was already in the non signaled state, it does nothing.

````cpp
void CEvent::Reset()
{
    m_EventHandle.store(NotSignaledValue);
}
````

### IsSignaled: checks if an event is signaled.

````cpp
bool CEvent::IsSignaled() const
{
    EVENT_SUPPORT_TYPE OldValue;

    if (IsManual())
        OldValue = m_EventHandle.load();
    else
        OldValue = m_EventHandle.exchange(NotSignaledValue);

    bool Result = (OldValue == SignaledValue);

    return Result;
}
````

### WaitSignaled: waits for an event to be signaled.

This method exists in two overloads. The second overload reports how long the method had to wait for the event to be signaled in the `observedDelay` output parameter.

````cpp
void CEvent::WaitSignaled()
{
    int ObservedDelay;
    WaitSignaled(ObservedDelay);
}

void CEvent::WaitSignaled(int& observedDelay)
{
    timespec Time =
    {
        0,
        1000L * 1000L // 1ms
    };

    observedDelay = 0;

    while (!IsSignaled())
    {
        timespec Remaining;
        nanosleep(&Time, &Remaining);

        observedDelay++;
    }
}
````
