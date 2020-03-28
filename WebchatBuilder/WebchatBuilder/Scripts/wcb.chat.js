/*!
 * web.chat.js
 * Copyright Qsect LLC
 * Contact support@qsect.com for licensing information
 */
(function ($) {
    var urlBase = "";
    if (typeof wcb_urlBase !== "undefined") {
        urlBase = wcb_urlBase;
    }

    $("#wcb-webchat-overlay").css("overflow-y", "visible");

    var connectionId = null;

    var showTime = $("#wcb-show-time").data("value");
    var inTestMode = $("#wcb-in-test-mode").data("value");
    var userName = $("#wcb-user-name").data("value");
    var profileName = $("#wcb-chat-profile-name").data("value");
    var launchText = $("#wcb-chat-launch-text").data("value");
    var tooltipText = $("#wcb-chat-tooltip-text").data("value");
    var launchIcon = $("#wcb-chat-launch-icon").data("value");
    var browserAlerts = $("#wcb-chat-browser-alerts").data("value");
    var showDisconnect = false;
    var showCustomInfoOnReload = $("#wcb-show-custom-info").data("value");
    var showCustomInfoOnLoad = $("#wcb-show-initial-info").data("value");

    var isMobile = false;
    if (typeof wcb_isMobileWidth !== "undefined") {
        isMobile = wcb_isMobileWidth;
    }

    var hasHeader = false;
    var wcbHeader = document.getElementById("wcb-chat-header");
    var headerHeight = 0;
    if (typeof wcbHeader !== "undefined" && wcbHeader !== null) {
        headerHeight = wcbHeader.offsetHeight;
        hasHeader = true;
    }

    var wcbChatDisconnect = document.getElementById("wcb-chat-disconnect");
    if (typeof wcbChatDisconnect !== "undefined" && wcbChatDisconnect !== null) {
        showDisconnect = true;
    }

    if (showDisconnect) {
        $("#wcb-chat-disconnect").show();
    }

    var resizeForKeyboard = false;

    var audioAlertsEnabled = false;
    var audioAlert = document.getElementById("wcb-audio-alert");
    if (typeof audioAlert !== "undefined" && audioAlert !== null) {
        audioAlertsEnabled = true;
    }

    var wcbTooltip = document.getElementById("wcb-launch-tooltip");
    var wcbTooltipSyleLeft = null;
    var wcbTooltipSyleRight = null;
    var wcbTooltipSyleBottom = null;

    if (typeof wcbTooltip !== "undefined" && wcbTooltip !== null) {
        wcbTooltipSyleLeft = document.getElementById("wcb-launch-tooltip").style.left;
        wcbTooltipSyleRight = document.getElementById("wcb-launch-tooltip").style.right;
        wcbTooltipSyleBottom = document.getElementById("wcb-launch-tooltip").style.bottom;
    }

    var wcbTooltipLeft = $("#wcb-launch-tooltip").css("left");
    var wcbTooltipRight = $("#wcb-launch-tooltip").css("right");
    var wcbTooltipBottom = $("#wcb-launch-tooltip").css("bottom");


    if ($("#wcb-webchat-launch-text").length && launchText.length > 0) {
        $("#wcb-webchat-launch-text").text(launchText);
        setTimeout(rePosTooltip, 500);
    }

    function rePosTooltip() {
        if (hasHeader && $("#wcb-launch-tooltip").length && !($(".wcb-launch-vertical").length)) {
            var right = wcbTooltipRight;
            var styleRight = wcbTooltipSyleRight;
            var left = wcbTooltipLeft;
            var styleLeft = wcbTooltipSyleLeft;
            var bottom = wcbTooltipBottom;
            var styleBottom = wcbTooltipSyleBottom;
            var width = $("#wcb-webchat-launch").width();
            if (bottom === "auto" || bottom === "0px" || styleBottom === "") {
                var lRight = $("#wcb-webchat-launch").css("right");
                var lLeft = $("#wcb-webchat-launch").css("left");
                if (left === "auto" || styleLeft === "" || lRight === "0px") {
                    var newWidth = width + parseInt(lRight.slice(0, -2)) + 5;
                    $("#wcb-launch-tooltip").css("right", newWidth + "px");
                }
                if (right === "auto" || styleRight === "" || lLeft === "0px") {
                    var newWidth = width + parseInt(lLeft.slice(0, -2)) + 5;
                    $("#wcb-launch-tooltip").css("left", newWidth + "px");
                }
            }
        }
    }

    if ($("#wcb-tooltip-text").length && tooltipText.length > 0) {
        $("#wcb-tooltip-text").text(tooltipText);
    }

    if (typeof wcb_integratedButton !== "undefined" && wcb_integratedButton !== null) {
        if (wcb_integratedButton && tooltipText.length > 0) {
            $("#wcb-webchat-launch").attr("title", tooltipText);
            $("#wcb-webchat-launch").addClass("wcb-state-resumable");
            $("#wcb-webchat-launch").removeClass("wcb-state-available");
        }
    }

    if ($("#wcb-webchat-launch-icon").length && launchIcon.length > 0) {
        $("#wcb-webchat-launch-icon").attr("src", launchIcon);
        $("#wcb-webchat-launch-icon").attr('alt', 'chat launch icon');
    }

    $.ajaxSetup({
        xhrFields: {
            withCredentials: true
        }
    });

    var initialTitle = document.title;
    var updatedTitle = initialTitle;
    var isFocused = true;
    var newMessagesReceived = 0;

    try {
        var target = document.querySelector("title");
        var observer = new MutationObserver(function(mutations) {
            if (document.title !== updatedTitle) {
                initialTitle = document.title;
                updatedTitle = document.title;
            }
        });
        var config = { subtree: true, characterData: true, childList: true };
        observer.observe(target, config);
    } catch (err) {
        console.log("MutationObserver not supported");
    }


    $(window).focus(function () {
        isFocused = true;
        if (browserAlerts) {
            setTimeout(function () {
                document.title = initialTitle;
            }, 100);
        }
        newMessagesReceived = 0;
    });

    $(window).blur(function () {
        isFocused = false;
    });


    var bodyHeight = $("#wcb-chat-body").prop("scrollHeight");
    var chatScrollHeight = 0;
    var signalrConnected = false;
    if (inTestMode) {
        enableSendButton();
    }

    //var tBody = document.getElementById('wcb-chat-body');
    var tEntry = document.getElementById("wcb-chat-entry").offsetHeight;
    var tHeight = $("#wcb-webchat-inner").height();//window.innerHeight;
    $("#wcb-chat-body").height(tHeight - (tEntry + headerHeight));

    function resizeChatBody() {
        var deviceHeight = window.innerHeight;
        var deviceWidth = window.innerWidth;
        var lgDiff = 227;
        var mdDiff = 157;
        var smDiff = 137;
        if (!$("#wcb-chat-header").length) {
            lgDiff = lgDiff - 65;
            mdDiff = mdDiff - 50;
            smDiff = smDiff - 50;
            $("#wcb-body").css("padding", "2%");
        }

        if ($("#wcb-webchat-inner").length) {
            var iHeight = $("#wcb-webchat-inner").height();
            var iWidth = $("#wcb-webchat-inner").width();
            if (iWidth <= 1000) {
                $("[id^='wcb-']").addClass("wcb-chat-1000");
                $("#wcb-chat-body").height((iHeight - lgDiff)-10);
            }
            if (iWidth <= 767) {
                $("[id^='wcb-']").addClass("wcb-chat-767");
                $("#wcb-chat-body").height((iHeight - mdDiff)-10);
            }
            if (iWidth <= 500) {
                $("[id^='wcb-']").addClass("wcb-chat-500");
                $("#wcb-chat-body").height((iHeight - smDiff)-20);
            }
            if (resizeForKeyboard) {
                var newHeight = $("#wcb-chat-body").height();
                $("#wcb-chat-body").height(newHeight*.25);
            }
        } else {
            var bHeight = deviceHeight - smDiff;
            if (deviceWidth > 767) {
                bHeight = deviceHeight - lgDiff;
            } else if (deviceWidth > 500) {
                bHeight = deviceHeight - mdDiff;
            }
            if (resizeForKeyboard) {
                bHeight = bHeight*.25;
            }
            if (inIframe()) {
                bHeight = bHeight - 20;
            }
            $("#wcb-chat-body").height(bHeight);
        }
        var bh = $("#wcb-chat-body").prop("scrollHeight");
        $("#wcb-chat-body").scrollTop(bh);
        resizeForKeyboard = false;
    }
    resizeChatBody();

    var entryHeight = $("#wcb-chat-entry").outerHeight();
    $("#wcb-chat-agent-typing").css("bottom", entryHeight + 15);

    $(window).resize(function () {
        resizeChatBody();
        entryHeight = $("#wcb-chat-entry").outerHeight();
        $("#wcb-chat-agent-typing").css("bottom", entryHeight + 15);
    });

    $("#wcb-chat-entry").on("click", "#wcb-chat-dotdotdot", function () {
        $("#wcb-chat-option-btn-wrapper").toggle();
    });

    $("#wcb-body").on("click", "#wcb-print-btn", function () {
        var transcriptUrl = urlBase + "/Chat/ChatTranscript?connectionId=" + connectionId;
        window.open(transcriptUrl);
    });

    $("#wcb-body").on("click", "#wcb-transcript-btn", function () {
        if (connectionId !== null) {
            var email = prompt("Please enter the email address to send the chat transcript to:");
            if (email !== null) {
                $.post(urlBase + "/Chat/RequestTranscriptEmail", { connectionId: connectionId, email: email }, function (response) {
                    if (response.success || false) {
                        alert("Chat Transcript Sent.");
                    } else {
                        alert("Unable to send chat transcript.");
                    }
                }).fail(function () {
                    alert("Unable to send chat transcript.");
                });
            }
        }
    });
           
    // Reference the auto-generated proxy for the hub.
    $.connection.hub.url = urlBase + "/signalr";
    var chat = $.connection.webChatHub;

    chat.client.addNewSystemMessageToPage = function (text) {
        //console.log("System Message Received: " + text);
        var html = '<div class="chat-system-message">' + text + "</div>";
        html = WcbAutoLinker.link(html);
        $("#wcb-chat-body").append(html);
        if ($("#wcb-webchat-launch").css("display") !== "none") {
            if ($("#wcb-webchat-overlay").css("display") === "none") {
                $("#wcb-webchat-overlay").show();
                wcb_rotateLaunchArrow("down");
            }
        }
        bodyHeight = $("#wcb-chat-body").prop("scrollHeight");
        $("#wcb-chat-body").stop().animate({ scrollTop: bodyHeight }, 400);
    }

    chat.client.enableSendButton = function () {
        enableSendButton();
    }

    chat.client.pauseWcbChat = function(isPaused) {
        pauseWcbChat(isPaused);
    }

    var agentJoined = false;
    chat.client.agentJoined = function (name) {
        if (showDisconnect) {
            $("#wcb-chat-disconnect").show();
        }
        //if (!agentJoined) {
            $("#wcb-chat-agent-name").text(name);
            enableSendButton();
            agentJoined = true;
        //}
    }
    chat.client.agentTyping = function (isTyping, name) {
        $("#wcb-chat-agent-name").text(name);
        if (!agentJoined) {
            enableSendButton();
            agentJoined = true;
        }
        if (isTyping) {
            $("#wcb-chat-agent-typing").show();
        } else {
            $("#wcb-chat-agent-typing").hide();
        }
    }
    var wcb_lastMessagePosted = "";
    // Create a function that the hub can call back to display messages.
    chat.client.addNewMessageToPage = function (name, text, imgsrc, direction, initials, messageId, dateSent) {
        if (wcb_lastMessagePosted === messageId) {
            //console.log("Duplicate message ignored");
            return;
        }
        wcb_lastMessagePosted = messageId;
        var html = "";
        if (direction === "out") {
            //console.log("Message Sent: " + text);
            var source = $("#sent-template").html();
            var template = WcbHandlebars.compile(source);
            if (showTime) {
                var date = new Date(dateSent);
                html = template({ name: name, text: text, imgsrc: imgsrc, initials: initials, time: date.toLocaleTimeString() });
            } else {
                html = template({ name: name, text: text, imgsrc: imgsrc, initials: initials });
            }
            html = WcbAutoLinker.link(html);
            $("#wcb-chat-body").append(html);
            //console.log("chat msg sent: " + text);
        } else {
            //console.log("Message Received: " + text);
            $("#wcb-chat-agent-typing").hide();
            if (!agentJoined) {
                enableSendButton();
                agentJoined = true;
                $("#wcb-chat-agent-name").text(name);
            }
            var source = $("#received-template").html();
            var template = WcbHandlebars.compile(source);
            
            if (!isFocused) {
                if (browserAlerts) {
                    newMessagesReceived++;
                    updatedTitle = initialTitle + " - " + newMessagesReceived + " New Messages";
                    document.title = updatedTitle;
                }
                if (audioAlertsEnabled) {
                    if (typeof audioAlert !== "undefined" && audioAlert !== null) {
                        try {
                            audioAlert.play();
                        } catch (err) {
                            console.log("Error playing alert.");
                        }
                    }
                }
            }

            if (showTime) {
                var date = new Date(dateSent);
                html = template({ name: name, text: text, imgsrc: imgsrc, initials: initials, time: date.toLocaleTimeString() });
            } else {
                html = template({ name: name, text: text, imgsrc: imgsrc, initials: initials });
            }
            html = WcbAutoLinker.link(html);
            $("#wcb-chat-body").append(html);
            if ($("#wcb-webchat-launch").css("display") !== "none") {
                if ($("#wcb-webchat-overlay").css("display") === "none") {
                    $("#wcb-webchat-overlay").show();
                    wcb_rotateLaunchArrow("down");
                }
            }
            //console.log("chat msg received: " + text);
        }
        bodyHeight = $("#wcb-chat-body").prop("scrollHeight");
        $("#wcb-chat-body").stop().animate({ scrollTop: bodyHeight }, 400);
    };

    chat.client.disconnected = function (allowRestart, isUserInitiated) {
        $("#wcb-chat-agent-typing").hide();
        agentJoined = false;
        if (isUserInitiated) {
            if (inIframe()) {
                window.top.postMessage("closeChat", "*");
            } else {
                wcb_hideChatOverlay();
            }
        } else {
            $("#wcb-chat-disconnect").hide();
        }
        if (!allowRestart) {
            $.connection.hub.stop();
            //$("#wcb-exit-btn").addClass("disabled");
            $("#wcb-chat-entry-btn").addClass("disabled");
            $("#wcb-chat-entry-text").prop("disabled", true);
        }
    }

    var errorText = $("#wcb-error").text();
    chat.client.serverReconnecting = function () {
        //console.log("Reconnecting to Server");
        $("#wcb-error").text("Chat Interrupted, Reconnecting...");
        $("#wcb-error").show();    
    }

    chat.client.serverReconnected = function () {
        //console.log("Reconnected to Server");
        $("#wcb-error").hide();    
        $("#wcb-error").text(errorText);
    }

    function enableSendButton() {
        if ($("#wcb-chat-entry-btn").hasClass("disabled")) {
            $("#wcb-chat-entry-btn").removeClass("disabled");
        }
    }

    chat.client.restartChat = function (message) {
        //console.log("Restarting Chat");
        var attributeNames = [];
        var attributeValues = [];
        var customInfo = "";

        if (typeof wcb_attributeNames !== "undefined") {
            attributeNames = wcb_attributeNames;
        }
        if (typeof wcb_attributeValues !== "undefined") {
            attributeValues = wcb_attributeValues;
        }
        if (typeof wcb_customInfo !== "undefined") {
            customInfo = wcb_customInfo;
        }
        //profileName
        if (!inTestMode) {
            $.post(urlBase + "/Chat/RestartDisconnectedSession", { connectionId: connectionId, profile: profileName, user: userName, customInfo: customInfo, attributeNames: attributeNames, attributeValues: attributeValues }, function (response) {
                if (response.success || false) {
                    chat.server.send(userName, message);
                    if (showDisconnect) {
                        $("#wcb-chat-disconnect").show();
                    }
                } else {
                    $("#wcb-error").show();
                }
            }).fail(function () {
                $("#wcb-error").show();
            });
        }
    }

    wcb_startNewChat();

    function wcb_startNewChat() {
        // Set initial focus to message input box.
        if (!isMobile) {
            $("#wcb-chat-entry-text").focus();
        }
        // Start the connection.
        //$.connection.hub.logging = true;
        $.connection.hub.start().done(function () {
            connectionId = $.connection.hub.id;
            if (!inTestMode) {
                $.post(urlBase + "/Chat/RegisterHubConnection", { connectionId: connectionId }, function(response) {
                    if (response.success || false) {
                        //enableSendButton();
                        if ((response.chatResumed || false) || (response.existingSession || false)) {
                            if (typeof wcb_customInfo !== "undefined" && wcb_customInfo.length > 0 && showCustomInfoOnReload) {
                                var customInfo = wcb_customInfo;
                                //console.log("Sending CustomInfo to Agent: " + customInfo);
                                setTimeout(function() {
                                    chat.server.sendCustomInfoToAgent(customInfo);
                                }, 1000);
                            }
                        } else if (typeof wcb_customInfo !== "undefined" && wcb_customInfo.length > 0 && showCustomInfoOnLoad) {
                                var customInfo = wcb_customInfo;
                                //console.log("Sending CustomInfo to Agent: " + customInfo);
                                setTimeout(function () {
                                    chat.server.sendCustomInfoToAgent(customInfo);
                                }, 1000);
                            }
                    } else {
                        $("#wcb-error").show();
                    }
                }).fail(function() {
                    $("#wcb-error").show();
                });
            }

            $("#wcb-chat-entry-text").wcbtyping({
                start: function(event, $elem) {
                    chat.server.typing(true);
                },
                stop: function(event, $elem) {
                    chat.server.typing(false);
                },
                delay: 1000
            });

        });

        $.connection.hub.stateChanged(function (change) {
            if (change.newState === $.signalR.connectionState.connected) {
                signalrConnected = true;
                connectionId = $.connection.hub.id;
            } else if (change.newState === $.signalR.connectionState.reconnecting) {
            } else if (change.newState === $.signalR.connectionState.disconnected) {
                signalrConnected = false;
            }
            console.log("Webchat Connection Active: " + signalrConnected);
        });
    }


    $("#wcb-body").on("click", "#wcb-chat-entry-btn", function () {
        if ($(this).hasClass("disabled")) {
            return false;
        }
        // Call the Send method on the hub.
        var message = $("#wcb-chat-entry-text").val();
        var content = message.replace(/[\n\r]/g, "");
        if (content.length > 0) {
            if (inTestMode) {
                chat.server.sendTest(userName, message);
            } else {
                chat.server.send(userName, message);
            }
            $("#wcb-chat-entry-text").val("");
            if (!isMobile) {
                $("#wcb-chat-entry-text").focus();
            }
        }
    });

    $("#wcb-body").on("click", "#wcb-exit-btn", function () {
        if (signalrConnected) {
            chat.server.disconnect($.connection.hub.id,false);
        } else {
            if (inIframe()) {
                window.top.postMessage("closeChat", "*");
            } else {
                wcb_hideChatOverlay();
            }
        }
    });

    $("#wcb-body").on("click", "#wcb-chat-disconnect", function () {
        $("#wcb-chat-agent-typing").hide();
        if (signalrConnected) {
            chat.server.disconnect($.connection.hub.id,true);
        }
        $(this).hide();
    });

    //iphone fix
    if ("ontouchstart" in window && isMobile) {
        $(window).scroll(function (e) {
            if ($("#wcb-webchat-overlay").css("display") !== "none") {
                $(window).scrollTop(0);
            }
        });
        setInterval(function() {
            if ($("#wcb-webchat-overlay").css("display") !== "none" && !$("#wcb-chat-entry-text").is(":focus")) {
                resizeChatBody();
            }
        }, 1000);
    }

    $("#wcb-body").on("focus", "#wcb-chat-entry-text", function (event) {
        if ("ontouchstart" in window && isMobile && hasHeader) {
            $("#wcb-webchat-overlay").css("position", "absolute");
            if ((navigator.userAgent.match(/iPhone/i)) || (navigator.userAgent.match(/iPod/i))) {
                resizeForKeyboard = true;
                setTimeout(resizeChatBody, 500);
            }
        }
    });

    $("#wcb-body").on("blur", "#wcb-chat-entry-text", function (event) {
        if ("ontouchstart" in window && isMobile && hasHeader) {
            $("#wcb-webchat-overlay").css("position", "fixed");
            if ((navigator.userAgent.match(/iPhone/i)) || (navigator.userAgent.match(/iPod/i))) {
                setTimeout(resizeChatBody, 500);
            }
        }
    });

    $("#wcb-body").on("keypress", "#wcb-chat-entry-text", function (event) {
        if (event.shiftKey === true && event.keyCode === 13) {
            return;
        }
        if (event.keyCode === 13) {
            var message = $("#wcb-chat-entry-text").val();
            message = message.replace(/[\n\r]/g, "");
            if (message.length > 0) {
                event.preventDefault();
                $("#wcb-chat-entry-btn").trigger("click");
            }
        }
    });

    $("body").on("click", "#wcb-webchat-launch", function () {
        if (signalrConnected) {
            if ($("#wcb-webchat-overlay").css("display") === "none") {
                //console.log("Resume Chat");
                chat.server.resumeChat();
                $("#wcb-webchat-overlay").show();
                if (wcb_collapsibleLauncher === false) {
                    $("#wcb-webchat-launch").hide();
                }
                resizeChatBody();
                bodyHeight = $("#wcb-chat-body").prop("scrollHeight");
                $("#wcb-chat-body").stop().animate({ scrollTop: bodyHeight }, 400);
            } else {
                if (wcb_collapsibleLauncher === true) {
                    pauseWcbChat(true);
                }
            }
        }
    });

    function pauseWcbChat(isPaused) {
        if (!isPaused) {
            chat.server.pauseChat();
        }
        $("#wcb-webchat-overlay").hide();
        if (wcb_collapsibleLauncher === false) {
            $("#wcb-webchat-launch").show();
        }
    }

    function inIframe() {
        try {
            return window.self !== window.top;
        } catch (e) {
            return true;
        }
    }
})(jQuery);