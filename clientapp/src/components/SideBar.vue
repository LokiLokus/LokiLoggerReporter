<template>
<ul class="navbar-nav bg-gradient-primary sidebar sidebar-dark accordion" id="accordionSidebar">
    <router-link class="sidebar-brand d-flex align-items-center justify-content-center" to="/home">
      <div class="sidebar-brand-icon rotate-n-15">
        <i class="fas fa-laugh-wink"></i>
      </div>
      <div class="my-logo sidebar-brand-text mx-3">
        BSS-PreAlpha
      </div>
    </router-link>

    <!-- User Area -->
    <hr class="sidebar-divider" />
    <div class="sidebar-heading">
        General
    </div>
    <li class="nav-item">
      <router-link class="nav-link" to="/">
        <font-awesome-icon icon="archway" />
        <span> Dashboard</span>
      </router-link>
    </li>
    <hr class="sidebar-divider" />
    <div class="sidebar-heading">
        Sources
    </div>
    <li class="nav-item">
      <router-link class="nav-link" to="/sources">
        <font-awesome-icon icon="archway" />
        <span> Sources</span>
      </router-link>
    </li>
    <li class="nav-item">
      <router-link class="nav-link" to="/loganalyzer">
        <font-awesome-icon icon="archway" />
        <span> Log Analyzer</span>
      </router-link>
    </li>
    <li class="nav-item">
      <router-link class="nav-link" to="/restusageanalyzer">
        <font-awesome-icon icon="archway" />
        <span> Rest Usage Analyzer</span>
      </router-link>
    </li>
</ul>
</template>
<script>
export default {};
</script>
