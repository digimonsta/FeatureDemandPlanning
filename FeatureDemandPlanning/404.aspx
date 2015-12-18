﻿<% Response.StatusCode = 404 %>


<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Page Not Found - Feature Demand Planning</title>

    <link href="/FeatureDemandPlanning/Content/Bootstrap/css/bootstrap.css" rel="stylesheet"/>
    <link href="/FeatureDemandPlanning/Content/bootstrap-slider.css" rel="stylesheet"/>
    <link href="/FeatureDemandPlanning/Content/css/site.css" rel="stylesheet"/>
    <link href="/FeatureDemandPlanning/Content/css/BrushedMetal.css" rel="stylesheet"/>

    <script src="/FeatureDemandPlanning/Content/Bootstrap/js/bootstrap.min.js"></script>
    <script src="/FeatureDemandPlanning/Scripts/bootstrap-slider.js"></script>
    
</head>
<body>
    <nav class="navbar navbar-inverse navbar-fixed-top">
        <div class="container-fluid">
            <div class="navbar-header">
                <button type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#bs-example-navbar-collapse-2">
                    <span class="sr-only">Toggle navigation</span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                </button>

                <a href="/FeatureDemandPlanning/">
                    <div class="navbar-brand metal linear">
                    <div style="margin-top:-3px">F.D.P</div>
                    <p>Powered by RADS</p>
                    </div>
                </a>

            </div>

            <div class="collapse navbar-collapse" id="bs-example-navbar-collapse-2">
                
                <ul class="nav navbar-nav pull-right">
                    <li>
                        <a href="/FeatureDemandPlanning/Admin">
                            <span class="glyphicon glyphicon-cog"></span> Settings
                        </a>
                    </li>
                </ul>
                <div class="nav navbar-nav navbar-right hidden-sm" style="position:relative;width:230px;height:60px;">
                    <img class="navbar-right" src="/FeatureDemandPlanning/Content/Images/Brand/jag_logo_transparent.png" style="position:absolute;left:0;bottom:0;" />
                    <img class="navbar-right" src="/FeatureDemandPlanning/Content/Images/Brand/lr_logo_v2_transparent.png" style="position:absolute;left:108px;bottom:0;" />
                </div>
            </div>
        </div>
    </nav>

    <div class="carousel-overlay">
        <div class="carousel-overlay-parent"></div>
    </div>

    <div class="container fill nopadding carousel-outer">
        <div id="backgroundCarousel" class="carousel slide">
            <div class="carousel-inner">
                <div class="active item">
                    <div class="fill" style="background-image:url(/FeatureDemandPlanning/Content/Images/Carousel/2.png);">
                        <div class="container">

                        </div>
                    </div>
                </div>
                <div class="item">
                    <div class="fill" style="background-image:url(/FeatureDemandPlanning/Content/Images/Carousel/3.png);">
                        <div class="container">

                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="col-xs-12">
        <div class="col-xs-12 hidden-xs inverse text-center" style="margin-bottom:15px">
            <h1 class="text-capitalize">Page Not Found</h1>
        </div>
        <div class="col-md-6 col-lg-4 col-md-offset-3 col-lg-offset-4 inverse home-button text-center">
            
            <p style="height:80px">The requested page could not be found. Go back with your browser or click the "Take me Home" button to continue.</p>
            <p><a class="btn btn-default col-md-10 col-md-offset-1" href="/FeatureDemandPlanning" role="button">Take me Home</a></p>
        </div>
    </div>
    
</body>
</html>
