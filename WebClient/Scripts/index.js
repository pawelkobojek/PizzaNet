$(document).ready(function () {

    pizzas = [];

    var scrollTo = function (selector) {
        $('html,body').animate({
            scrollTop: $(selector).offset().top
        }, 'slow');
    }


    $('#pizza-details').hide();
    $('.order-list').hide();

    $('.recipe-container').mouseenter(function () {
        $(this).animate({
            height: '+=15px'
        });
    });

    $('.recipe-container').mouseleave(function () {
        $(this).animate({
            height: '-=15px'
        });
    });

    $('.recipe-container').click(function () {
        var wasSelected = $(this).hasClass('checked');
        $('.recipe-container').removeClass('checked');
        $('.ingredient-container').removeClass('normal');
        $('.ingredient-container').removeClass('extra');

        if (!wasSelected) {
            $(this).addClass('checked');
            $('#pizza-details').show(500, function () {
                varMaxSizeHeight = $('.pizza-info').outerHeight(false);
                currentSizeHeight = varMaxSizeHeight / 3;
                varDiffer = currentSizeHeight;
                $('.size-chooser').animate({
                    height: currentSizeHeight + 'px'
                });
                $('.size-chooser').html('S<br/>m<br/>a<br/>l<br/>l');

                $('.size-chooser').removeClass('big');
                $('.size-chooser').removeClass('normal');

                //$('html,body').animate({
                scrollTo(".main-content");
                //    scrollTop: $(".main-content").offset().top
                //}, 'slow');
            });

            var selected = $(this);

        } else {
            $('#pizza-details').hide(500);
        }

        // Code below is responsible for pre-selecting proper ingredients 
        var array = $(this).children('.recipe-ingredients').children('*');

        for (var i = 0; i < array.length; i++) {
            var ingredientId = $(array[i]).attr('data-ingredient-id');
            $("div[data-ingredient-id='" + ingredientId + "']").addClass('normal');
        }
    });

    $('.ingredient-container').mouseenter(function () {
        $(this).animate({
            width: '-=10px'
        });
    });

    $('.ingredient-container').mouseleave(function () {
        $(this).animate({
            width: '+=10px'
        });
    });

    $('.ingredient-container').click(function () {

        var wasNormal = $(this).hasClass('normal');
        var wasExtra = $(this).hasClass('extra');

        if (wasNormal) {
            $(this).removeClass('normal');
            $(this).addClass('extra');
        } else if (wasExtra) {
            $(this).removeClass('extra');
        } else {
            $(this).addClass('normal');
        }
    });

    $('.size-chooser').mouseenter(function () {
        if ($(this).hasClass('big')) {
            $(this).animate({
                height: '-=10px'
            });
        } else {
            $(this).animate({
                height: '+=10px'
            });
        }
    });

    $('.size-chooser').mouseleave(function () {
        if ($(this).hasClass('big')) {
            $(this).animate({
                height: varMaxSizeHeight + 'px'
            });
        } else {
            $(this).animate({
                height: '-=10px'
            });
        }
    });

    $('.size-chooser').click(function () {
        var elem = $(this);
        if (elem.hasClass('normal')) {
            currentSizeHeight = varMaxSizeHeight;

            elem.animate({
                height: currentSizeHeight - 10 + 'px'
            });
            elem.removeClass('normal');
            elem.addClass('big');
            elem.html('B<br/>i<br/>g<br/>');

        } else if (elem.hasClass('big')) {
            currentSizeHeight = varMaxSizeHeight / 3;

            elem.animate({
                height: currentSizeHeight + 'px'
            });

            elem.html('S<br/>m<br/>a<br/>l<br/>l');

            elem.removeClass('big');
        } else {
            currentSizeHeight += varDiffer;
            elem.animate({
                height: currentSizeHeight + 'px'
            });
            elem.addClass('normal');

            elem.html('N<br/>o<br/>r<br/>m<br/>a<br/>l');
        }
    });

    $('.add-pizza').mousedown(function () {
        $(this).css('background-color', '#a6dbed');
    });

    $('.add-pizza').mouseup(function () {
        $(this).css('background-color', '#42BDE4');
    });

    $('form[data-pizza-ajax="true"]').submit(function () {

        var ings = [];
        var quants = [];
        $('.ingredient-container').each(function () {
            if ($(this).hasClass('normal')) {
                ings.push($(this).attr('data-ingredient-id'));
                quants.push('normal');
            }
            if ($(this).hasClass('extra')) {
                ings.push($(this).attr('data-ingredient-id'));
                quants.push('extra');
            }
        });

        var sizeVal;
        if ($('.size-chooser').hasClass('normal'))
            sizeVal = 'normal';
        else if ($('.size-chooser').hasClass('big'))
            sizeVal = 'big';
        else
            sizeVal = 'small';

        var objectToSend = {
            ingredients: ings,
            quantities: quants,
            size: sizeVal
        };


        var options = {
            url: $(this).attr("action"),
            type: $(this).attr("method"),
            traditional: true,
            data: objectToSend
        };

        pizzas.push(objectToSend);

        $.ajax(options).done(function (data) {
            //$('.order-list').html(data);
            $('.order-list').show(500);
            $('.orders').append(data);

            // TODO: Liczenie ceny
            //var price = 0;
            //$('.order-list').each(function (index, element) {
            //    alert(price);
            //    price += parseFloat($(element).attr('data-price'));
            //});

            //$('.minor-sub-header').text(price);

            scrollTo('.order-list');
        });

        return false;
    });

    $('#make-order').submit(function () {

        var options = {
            url: $(this).attr("action"),
            type: $(this).attr("method"),
            data: JSON.stringify(pizzas),
            dataType: 'json',
            contentType: 'application/json; charset=UTF-8'
        };

        $.ajax(options).complete(function (data) {
            pizzas = [];
            document.location = options.url;
        });

        return false;
    });
});