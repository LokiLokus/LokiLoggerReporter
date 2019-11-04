<template>
<div>
    <div id="wrapper">
      <side-bar/>
      <!-- Content Wrapper -->
      <div id="content-wrapper">
        <!-- Main Content -->
        <div id="content">
          <top-bar/>
          <router-view />
        </div>
        <!-- Footer -->
        <footer class="sticky-footer bg-white">
          <div class="container">
          </div>
        </footer>
      </div>
    </div>
  </div>
</template>
<style lang="scss"></style>
<script>
import SideBar from '@/components/SideBar.vue';
import TopBar from '@/components/TopBar.vue';
export default {
  components: {
      SideBar,
      TopBar,
  },
};
</script>
