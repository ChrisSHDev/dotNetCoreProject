(function ($) {
        function Rate() {
            var $this = this;
            function initialize() {
                $(".star").click(function () {
                    $(".star").removeClass('active');
                    $(this).addClass('active');
                    var starValue = $(this).data("value");
                    $("#Rate").val(starValue);
                });
            }
            $this.init = function () {
                initialize();
            };
        }
        $(function () {
            var self = new Rate();
    self.init();
});
}(jQuery));
