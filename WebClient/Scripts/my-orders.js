$(document).ready(function () {

    $('.order-details-container').mouseenter(function () {
        $(this).animate({
            width: '+=15px'
        });
    });

    $('.order-details-container').mouseleave(function () {
        $(this).animate({
            width: '-=15px'
        });
    });

    $('.order-details-container').click(function () {
        var clickedItem = $(this);

        if (clickedItem.hasClass('.expanded-order-details-container')) {

            clickedItem.animate({
                width: "50%",
                height: 25 + 'px'
            }, function () {
                clickedItem.children('*').remove('div');
                clickedItem.removeClass('.expanded-order-details-container');
            });
            return true;
        }
        var orderId = $(this).attr('data-order-id');
        var options = {
            url: "/Home/GetOrderInfo",
            type: 'GET',
            data: { id: orderId }
        }

        $.ajax(options).done(function (data) {
            var $newHTML = $('<div style="position : absolute; left : -9999px;">' + data + '</div>').appendTo('body'),
            desiredHeight = $newHTML.outerHeight() + 10;
            clickedItem.append(data);
            clickedItem.addClass('.expanded-order-details-container');
            clickedItem.animate({
                width: '100%',
                height: desiredHeight + 'px'
            });

            $newHTML.remove();
        });

        return true;
    });
});