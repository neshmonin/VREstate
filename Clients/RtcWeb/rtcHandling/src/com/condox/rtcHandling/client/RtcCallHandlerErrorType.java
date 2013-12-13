package com.condox.rtcHandling.client;

public enum RtcCallHandlerErrorType {
	FailureToSendMessage,
	FailureToReceiveMessage,
	FailureToGetUserMedia,
	FailureToCreateAnswer,
	FailureToCreateOffer,
	DataChannelFailure,
	BadCallerState // start method was called on both sides
}