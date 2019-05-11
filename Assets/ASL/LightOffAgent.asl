+light_on(lamp) : true
	<- .send(lamp, achive,light_off(lamp)).

//-light_on(lamp) : true
//  <- .print("Thanks for turn off the lamp!").

