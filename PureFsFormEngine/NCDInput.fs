namespace NCurGameEngine
module NCDInput =
  open SharpDX.DirectInput
  let input = new DirectInput()
  type StickErr =
    |NotInited
    |Disconnected

  let initByGUID guid (stick:Joystick option) =
    match stick with
      |Some j->j.Dispose()
      |None -> ()
    new Joystick(input, guid);

  let center = 30000
  let isXMin (state :JoystickState) =
    //0 < state.X && 
    state.X < center
  let isXMax (state :JoystickState) =
    34000 < state.X && state.X < 66000
  let isYMin (state :JoystickState) =
    //0 < state.Y && 
    state.Y < center
  let isYMax (state :JoystickState) =
    34000 < state.Y && state.Y < 66000

  let init() =
    let findDevices (dtype:DeviceType) = [for i in input.GetDevices(dtype, DeviceEnumerationFlags.AllDevices)->i]
    let stick = findDevices DeviceType.Joystick
    let pad = findDevices DeviceType.Gamepad
    initByGUID pad.Head.InstanceGuid None

  let update(stick:Joystick) =
    try 
      stick.Acquire()
      stick.Poll()
    // 抜けた
    with e -> ()// stick.Dispose()
    let state = stick.GetCurrentState()
    state

