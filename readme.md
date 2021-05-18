# Yonder Space Combat

This is the combat simulator for a programming game, played through a Discord bot in the [Cylon](https://discord.gg/Dcn7BG4) server. Spaceships are programmed in the Yolol language and then the bot automatically runs ships against each other to generate rankings.

## Folder Structure

Fleets are defined by a specific folder structure:

```
> RootFolder
 > ShipName
   > script1.yolol
   > script2.yolol
  > data
   > somefile.txt
```

 - `ShipName` defines a new ship. The local simulation supports an unlimited number of ships, but the online competition only supports single ship fleets.
 - `ShipName/script1.yolol` is a script which will run on the ship. Every `*.yolol` file in this folder will be turned into a Yolol chip. Every chip will execute every tick of the sim.
 - `data/somefile.txt` contains a string of text, it will be loaded into the `:somefile` variable when the sim starts.

## Run Locally

To run the simulation locally you should download `SpaceShipCombatSimulator.exe` and pass to it two arguments:

> `./SpaceShipCombatSimulator.exe -a path/to/fleet/1 -b path/to/fleet/2`

## Submit A Fleet

To submit a fleet to complete against other fleets you should submit it to the `Referee` bot by dragging and dropping the zipped folder into discord with the message:

> `>fleet submit name goes here`

If you submit a fleet with a name that already exists your new submission will replace that fleet.

# Devices

The game includes many devices which run on ships and missiles. Most ships **will not** need to interact with all of these devices!

Certain fields are read-only (**R**), other fields are write-only (**W**), some fields are both readable and writeable (**RW**).

## Ship

todo: link to the device info for each device
 - [Yolol Chip](#yolol-chip) (one per script)
 - [Radiation Sensor](#radiation-sensor)
 - [Clock](#clock)
 - [Gyroscope](#gyroscope)
 - [Positioning Device](#positioning-device)
 - [Fuel Tank](#fuel-tank)
 - [Engine](#engine)
 - [Momentum Wheels](momentum-wheels)
 - [Turret](#turret) (x4)
 - [Missile Launcher](#missile-launcher)
 - [Radar Scanner](#radar-scanner)
 - [Radio](#radio)
 - [Navigation Lights](#light)

## Missiles

 - [Yolol Chip](#yolol-chip) (just one)
 - [Clock](#clock)
 - [Gyroscope](#gyroscope)
 - [Positioning Device](#positioning-device)
 - [Fuel Tank](#fuel-tank)
 - [Engine](#engine)
 - [Momentum Wheels](momentum-wheels)
 - [Warhead](#warhead)
 - [Radar Scanner](#radar-scanner) (Fixed)
 - [Radio](#radio)

### Yolol Chip

The Yolol chip device runs a Yolol script, line by line. On ships every chip is run every single tick. On missiles there is just one chip.

### Radiation Sensor

Space is filled with deadly radiation which slowly kills the crew of the ship. Radiation gets worse the further away the ships travels from the middle of the battle zone.

 - `:cosmic_radiation` (**R**) - reads the current radiation level.

### Clock

Measures the time in seconds.

 - `:clock` - elapsed time since entity was created (in seconds).

### Gyroscope

Measures the orientation and angular velocity. The forward vector of the ship can be found by transforming `(0, 0, -1)` by this quaternion. Also measures the angular velocity.

 - `:gyro_w` (**R**) - **w** component of quaternion.
 - `:gyro_x` (**R**) - **x** component of quaternion.
 - `:gyro_y` (**R**) - **y** component of quaternion.
 - `:gyro_z` (**R**) - **z** component of quaternion.
 - `:angular_vel_x` (**R**) - **x** component of angular velocity.
 - `:angular_vel_y` (**R**) - **y** component of angular velocity.
 - `:angular_vel_z` (**R**) - **z** component of angular velocity.

### Positioning Device

Measures the acceleration, velocity and position. Position starts off at `(0,0,0)` wherever the entity is first created.

 - `:accel_x` (**R**) - **x** component of acceleration.
 - `:accel_y` (**R**) - **y** component of acceleration.
 - `:accel_z` (**R**) - **z** component of acceleration.
 - `:vel_x` (**R**) - **x** component of velocity.
 - `:vel_y` (**R**) - **y** component of velocity.
 - `:vel_z` (**R**) - **z** component of velocity.
 - `:pos_x` (**R**) - **x** component of position.
 - `:pos_y` (**R**) - **y** component of position.
 - `:pos_z` (**R**) - **z** component of position.

### Fuel Tank

Stores fuel which is burned by the engines. On space ships fuel also serves as "health" - hits explosions will remove fuel and when the ship is out of fuel it dies. Missiles do _not_ die when they are out of fuel.

 - `:fuel` (**R**) - total amount of fuel currently in the tank.

### Engine

Pushes the entity forward and consumes fuel.

 - `:throttle` (**W**) - A value from 0 to 1 which sets the engine power.

### Turret

The space ship has large turret mounted guns capable of firing nuclear warheads, each nuclear warhead has a timed fuse which sets when it will detonate. Turrets can turn left/right on their base (bearing) and point up/down (elevation). When the turret target angles are changed the turret takes some time to move around to that position. The ship has four turrets, each turret variable has the index of the turret (0 to 3) added to the end.

 - `:gun_bearing_0` (**R**) - current bearing of gun 0.
 - `:gun_elevation_0` (**R**) - current elevation of gun 0.
 - `:gun_fuse_0` (**W**) - fuse time (seconds) to use for shells fired from gun 0.
 - `:gun_ready_0` (**R**) - indicates if gun 0 is ready to fire.
 - `:gun_trigger_0` (**RW**) - sets how many shells should be fired from this turret. Every time a shell is fired this value will decrease by one.
 - `:tgt_gun_bearing_0` (**W**) - set the target bearing for turret 0.
 - `:tgt_gun_elevation_0` (**W**) - set the target elevation for turret 0.

### Missile Launcher

The space ship has a missile launcher with a limit number of missiles. Missiles are capable of independent flight and can detonate themselves instantly.

 - `:missile_ammo` (**R**) - The number of unfired missiles remaining.
 - `:missile_code` (**W**) - The code to execute on the next missile to fire.
 - `:missile_ready` (**R**) - Indicates if the missile launcher is ready to fire.
 - `:missile_trigger` (**RW**) - Sets how many missiles should be fired. Every time a missile is fired this value will decrease by one.

### Radar Scanner

Ships and missiles have an active radar scanner which can detect other things in space. On space ships the radar scanner can be steered to look in any direction and with any beam angle up to 80 degrees. On missiles the radar is a fixed forward facing 15 degree beam.

#### Ship Only
 - `:radar_beam_angle` (**W**) (Ship Only) - Set the beam angle in degrees.
 - `:radar_bearing` (**W**) (Ship Only) - Set the bearing of the beam.
 - `:radar_elevation` (**W**) (Ship Only) - Set the elevation of the beam.
 - `:radar_beam_range` (**W**) (Ship Only) - Set the range of the radar scan.

#### Ship And Missile
 - `radar_trigger` (**RW**) - trigger a new scan when truthy. Field will be set to zero.
 - `:radar_count` (**R**) - Number of items detected in the previous scan.
 - `:radar_idx` (**W**) - Index of the item to fetch information for.
 - `:radar_out_dist` (**R**) - Distance to the item indicated by `radar_idx`.
 - `:radar_out_type` (**R**) - Type of the item indicated `radar_idx` as a string.
 - `:radar_out_id` (**R**) - Unique ID of the item indicated `radar_idx` as a string.

### Radio

Every team has a perfectly secure radio channel which can transmit strings - there is no way for enemies to eavesdrop or even interfere with radio messages. However, if two allied ships transmit at the same time the message will be scrambled.

 - `:radio_send` (**W**) - if this field is a non-empty string it will be transmitted.
 - `:radio_recv` (**RW**) - the message sent last tick is _added_ to this field (string concatenation) every tick.

### Warhead

Missiles are fitted with a nuclear warhead which instantly destroys the missile when detonated.

 - `:self_destruct_prime` (**W**) - Activate the warhead. The trigger does nothing unless this is set to a truthy value.
 - `:self_destruct_trigger` (**W**) - Instantly detonate the warhead when set to a truthy value. Does nothing unless the trigger is primed.

### Light

The space ship is fitted with a large forward facing light.

 - `:light` (**W**) - Boolean value indicating if the light is on or off.