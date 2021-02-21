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

  let lower = 34000
  let center = 30000
  let isXMin (state :JoystickState) =
    state.X < center
  let isXMax (state :JoystickState) =
    lower < state.X
  let isYMin (state :JoystickState) =
    state.Y < center
  let isYMax (state :JoystickState) =
    lower < state.Y
  let isRXMin (state :JoystickState) =
    state.Z < center
  let isRYMin (state :JoystickState) =
    state.RotationZ < center

  let isRXMax (state :JoystickState) =
    lower < state.Z
  let isRYMax (state :JoystickState) =
    lower < state.RotationZ

  let A(state :JoystickState) =
    state.Buttons.[2]
  let B(state :JoystickState) =
    state.Buttons.[1]
  //let up 0 right 9000 down 18000 left 27000

  let init() =
    let findDevices (dtype:DeviceType) = [for i in input.GetDevices(dtype, DeviceEnumerationFlags.AllDevices)->i]
    let stick = findDevices DeviceType.Joystick
    let pad = findDevices DeviceType.Gamepad
    let joy = initByGUID pad.Head.InstanceGuid None
    //joy.GetObjects()
    joy

  let update(stick:Joystick) =
    try 
      stick.Acquire()
      stick.Poll()
    // 抜けた
    with e -> ()// stick.Dispose()
    let state = stick.GetCurrentState()
    state

