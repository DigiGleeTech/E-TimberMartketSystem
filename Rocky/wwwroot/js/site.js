    // Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.    $(document).ready(function(){
$(document).ready(function () {
    var ccErrorNo = 0;
    var ccErrors = new Array()

    ccErrors[0] = "Unknown card";
    ccErrors[1] = "No card number provided";
    ccErrors[2] = "Credit card number is in invalid format";
    ccErrors[3] = "Credit card number is invalid";
    ccErrors[4] = "Credit card number has an inappropriate number of digits";
    ccErrors[5] = "Warning! This credit card number is associated with a scam attempt";

    function checkCreditCard(cardnumber, cardname) {

        // Array to hold the permitted card characteristics
        var cards = new Array();

        // Define the cards we support. You may add addtional card types as follows.

        //  Name:         As in the selection box of the form - must be same as user's
        //  Length:       List of possible valid lengths of the card number for the card
        //  prefixes:     List of possible prefixes for the card
        //  checkdigit:   Boolean to say whether there is a check digit

        cards[0] = {
            name: "Visa",
            length: "13,16",
            prefixes: "44",
            checkdigit: true
        };
        cards[1] = {
            name: "MasterCard",
            length: "16",
            prefixes: "53,99,55",
            checkdigit: true
        };
        // Establish card type
        var cardType = -1;
        for (var i = 0; i < cards.length; i++) {

            // See if it is this card (ignoring the case of the string)
            if (cardname.toLowerCase() == cards[i].name.toLowerCase()) {
                cardType = i;
                break;
            }
        }

        // If card type not found, report an error
        if (cardType == -1) {
            ccErrorNo = 0;
            return false;
        }

        // Ensure that the user has provided a credit card number
        if (cardnumber.length == 0) {
            ccErrorNo = 1;
            return false;
        }

        // Now remove any spaces from the credit card number
        cardnumber = cardnumber.replace(/\s/g, "");

        // Check that the number is numeric
        var cardNo = cardnumber
        var cardexp = /^[0-9]{13,19}$/;
        if (!cardexp.exec(cardNo)) {
            ccErrorNo = 2;
            return false;
        }

        // Now check the modulus 10 check digit - if required
        if (cards[cardType].checkdigit) {
            var checksum = 0;                                  // running checksum total
            var mychar = "";                                   // next char to process
            var j = 1;                                         // takes value of 1 or 2

            // Process each digit one by one starting at the right
            var calc;
            for (i = cardNo.length - 1; i >= 0; i--) {

                // Extract the next digit and multiply by 1 or 2 on alternative digits.
                calc = Number(cardNo.charAt(i)) * j;

                // If the result is in two digits add 1 to the checksum total
                if (calc > 9) {
                    checksum = checksum + 1;
                    calc = calc - 10;
                }

                // Add the units element to the checksum total
                checksum = checksum + calc;

                // Switch the value of j
                if (j == 1) { j = 2 } else { j = 1 };
            }

            // All done - if checksum is divisible by 10, it is a valid modulus 10.
            // If not, report an error.
            if (checksum % 10 != 0) {
                ccErrorNo = 3;
                return false;
            }
        }

        // Check it's not a spam number
        if (cardNo == '5490997771092064') {
            ccErrorNo = 5;
            return false;
        }

        // The following are the card-specific checks we undertake.
        var LengthValid = false;
        var PrefixValid = false;
        var undefined;

        // We use these for holding the valid lengths and prefixes of a card type
        var prefix = new Array();
        var lengths = new Array();

        // Load an array with the valid prefixes for this card
        prefix = cards[cardType].prefixes.split(",");

        // Now see if any of them match what we have in the card number
        for (i = 0; i < prefix.length; i++) {
            var exp = new RegExp("^" + prefix[i]);
            if (exp.test(cardNo)) PrefixValid = true;
        }

        // If it isn't a valid prefix there's no point at looking at the length
        if (!PrefixValid) {
            ccErrorNo = 3;
            return false;
        }

        // See if the length is valid for this card
        lengths = cards[cardType].length.split(",");
        for (j = 0; j < lengths.length; j++) {
            if (cardNo.length == lengths[j]) LengthValid = true;
        }

        // See if all is OK by seeing if the length was valid. We only check the length if all else was 
        // hunky dory.
        if (!LengthValid) {
            ccErrorNo = 4;
            return false;
        };

        // The credit card is in the required format.
        return true;
    }
   
    function testCreditCard() {
        alert(333);
        myCardNo = document.getElementById('CardNumber').value;
        myCardType = document.getElementById('CardType').value;
        if (checkCreditCard(myCardNo, myCardType)) {
            alert("Credit card has a valid format")
        } else {
            alert(ccErrors[ccErrorNo])
        };
    }
});




