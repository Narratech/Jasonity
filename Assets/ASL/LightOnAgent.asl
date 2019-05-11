+light_off(lamp) : true
	<- .send(lamp, achive,light_on(lamp)).

//-light_off(lamp) : true
//  <- .print("Thanks for turn on the lamp!").
